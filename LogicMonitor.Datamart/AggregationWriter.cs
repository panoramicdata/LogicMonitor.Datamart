using LogicMonitor.Datamart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogicMonitor.Datamart
{
	internal static class AggregationWriter
	{
		internal static string TableNamePrefix = "DeviceDataSourceInstanceAggregatedData";

		public static string GetTableName(DateTimeOffset start)
			=> $"{TableNamePrefix}_{start.UtcDateTime.ToString("yyyyMMdd")}";

		/// <summary>
		/// Create a new aggregation table for a new day
		/// </summary>
		internal static async Task<string> EnsureTableExistsAsync(SqlConnection sqlConnection, DateTimeOffset start)
		{
			if (sqlConnection == null)
			{
				throw new ArgumentNullException(nameof(sqlConnection));
			}

			var tableName = GetTableName(start);
			var tableCreationSql = @"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='" + tableName + @"' and xtype='U')
	CREATE TABLE [" + tableName + @"](
		[Id][int] IDENTITY(1, 1) NOT NULL,
		[PeriodStart] [smalldatetime] NOT NULL,
		[PeriodEnd] [smalldatetime] NOT NULL,
		[DeviceDataSourceInstanceId] [int] NOT NULL,
		[DataPointId] [int] NOT NULL,
		[Min] [float] NULL,
		[Max] [float] NULL,
		[Sum] [float] NOT NULL,
		[SumSquared] [float] NOT NULL,
		[DataCount] [int] NOT NULL,
		[NoDataCount] [int] NOT NULL,
		CONSTRAINT[PK_" + tableName + @"] PRIMARY KEY CLUSTERED ( [Id] ASC )
		WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON ) ON[PRIMARY]
	) ON[PRIMARY]";

			using (var command = new SqlCommand(tableCreationSql, sqlConnection))
			{
				await command.ExecuteNonQueryAsync().ConfigureAwait(false);
			}
			return tableName;
		}

		/// <summary>
		/// Drop an aggregation table for a given period
		/// </summary>
		internal static async Task DropTableAsync(
			DbContextOptions<Context> dbContextOptions,
			DateTimeOffset start,
			ILogger logger)
		{
			if (dbContextOptions == null)
			{
				throw new ArgumentNullException(nameof(dbContextOptions));
			}

			using (var dbContext = new Context(dbContextOptions))
			{
				var tableName = GetTableName(start);
				logger.LogDebug($"Dropping table {tableName}");
				var tableCreationSql = "DROP TABLE [" + tableName + "]";
#pragma warning disable EF1000 // Possible SQL injection vulnerability. - No externally provided data
				await dbContext.Database.ExecuteSqlCommandAsync(tableCreationSql).ConfigureAwait(false);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
			}
		}

		/// <summary>
		/// Get all tables starting with the aggregation table prefix
		/// </summary>
		internal static async Task<List<string>> GetTablesAsync(DbContextOptions<Context> dbContextOptions)
		{
			if (dbContextOptions == null)
			{
				throw new ArgumentNullException(nameof(dbContextOptions));
			}

			var tableNames = new List<string>();
			using (var dbContext = new Context(dbContextOptions))
			{
				using (var connection = dbContext.Database.GetDbConnection())
				{
					connection.Open();
					using (var command = connection.CreateCommand())
					{
						command.CommandText = $"select name from sys.Tables where name like '{TableNamePrefix}%' order by name";
						using (var reader = await command.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (reader.Read())
								{
									tableNames.Add(reader.GetString(0));
								}
							}
						}
					}
				}
			}
			return tableNames;
		}

		internal static async Task WriteAggregations(
			SqlConnection sqlConnection,
			int deviceDataSourceInstanceId,
			DateTimeOffset key,
			IEnumerable<DeviceDataSourceInstanceAggregatedDataBulkWriteModel> aggregations,
			ILogger logger)
		{
			var tableName = await EnsureTableExistsAsync(sqlConnection, key);

			var aggregationCount = aggregations.Count();

			var stopwatch = Stopwatch.StartNew();
			logger.LogTrace($"Preparing DataTable for {aggregationCount} aggregations...");

			// Prep the data into a DataTable, setting initial structure from the database
			var table = new DataTable();
			using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM {tableName}", sqlConnection))
			{
				adapter.Fill(table);
			}

			// Populate the DataTable
			foreach (var aggregation in aggregations)
			{
				var row = table.NewRow();
				row["PeriodStart"] = aggregation.PeriodStart;
				row["PeriodEnd"] = aggregation.PeriodEnd;
				row["DeviceDataSourceInstanceId"] = aggregation.DeviceDataSourceInstanceId;
				row["DataPointId"] = aggregation.DataPointId;
				row["Min"] = aggregation.Min;
				row["Max"] = aggregation.Max;
				row["Sum"] = aggregation.Sum;
				row["SumSquared"] = aggregation.SumSquared;
				row["DataCount"] = aggregation.DataCount;
				row["NoDataCount"] = aggregation.NoDataCount;
				table.Rows.Add(row);
			}

			var lastAggregationHourWrittenUtc = aggregations.Max(a => a.PeriodEnd);
			logger.LogTrace($"Preparing DataTable for {aggregationCount} aggregations complete after {stopwatch.ElapsedMilliseconds:N0}ms.");
			stopwatch.Restart();

			using (var transaction = sqlConnection.BeginTransaction())
			{
				stopwatch.Restart();
				logger.LogTrace($"Bulk writing {aggregationCount} aggregations...");
				// Write out the data as part of a transaction
				using (var bulk = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
				{
					bulk.DestinationTableName = tableName;
					await bulk.WriteToServerAsync(table).ConfigureAwait(false);
				}
				logger.LogTrace($"Bulk writing {aggregationCount} aggregations complete after {stopwatch.ElapsedMilliseconds:N0}ms.");

				stopwatch.Restart();
				logger.LogTrace($"Setting progress for DDSI {deviceDataSourceInstanceId} to {lastAggregationHourWrittenUtc}...");
				// Update the progress as part of a transaction
				await WriteProgressBoundaryAsync(sqlConnection, deviceDataSourceInstanceId, lastAggregationHourWrittenUtc, transaction);
				logger.LogTrace($"Setting progress for DDSI {deviceDataSourceInstanceId} to {lastAggregationHourWrittenUtc} complete after {stopwatch.ElapsedMilliseconds:N0}ms.");

				stopwatch.Restart();
				logger.LogTrace("Committing transaction...");
				transaction.Commit();
				logger.LogTrace($"Committing transaction complete after {stopwatch.ElapsedMilliseconds:N0}ms");
			}
		}

		internal static async Task WriteProgressBoundaryAsync(SqlConnection sqlConnection, int deviceDataSourceInstanceId, DateTime lastAggregationHourWrittenUtc, SqlTransaction transaction)
		{
			const string sql = "update DeviceDataSourceInstances set LastAggregationHourWrittenUtc=@LastAggregationHourWrittenUtc where id=@Id";
			using (var command = new SqlCommand(sql, sqlConnection, transaction))
			{
				command.Parameters.AddWithValue("@LastAggregationHourWrittenUtc", lastAggregationHourWrittenUtc);
				command.Parameters.AddWithValue("@Id", deviceDataSourceInstanceId);
				await command.ExecuteNonQueryAsync().ConfigureAwait(false);
			}
		}

		internal static async Task PerformAgingAsync(
			DbContextOptions<Context> dbContextOptions,
			int countAggregationDaysToRetain,
			ILogger logger)
		{
			var existingTables = await GetTablesAsync(dbContextOptions).ConfigureAwait(false);

			// Determine which tables require aging
			var tablesToRemove = DetermineTablesToAge(existingTables, countAggregationDaysToRetain);
			if (tablesToRemove.Count > 0)
			{
				using (var context = new Context(dbContextOptions))
				using (var sqlConnection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
				{
					await sqlConnection.OpenAsync();
					using (var command = new SqlCommand(string.Empty, sqlConnection))
					{
						foreach (var tableName in tablesToRemove)
						{
							logger.LogInformation($"Aging out table {tableName}");
							command.CommandText = "drop table " + tableName;
							await command.ExecuteNonQueryAsync().ConfigureAwait(false);
						}
					}
				}
			}
		}

		internal static List<string> DetermineTablesToAge(List<string> existingTables, int countAggregationDaysToRetain)
		{
			var ageBoundary = DateTimeOffset.UtcNow.Date.AddDays(-countAggregationDaysToRetain).ToString("yyyyMMdd");
			return existingTables.Where(t => string.CompareOrdinal(t, TableNamePrefix.Length + 1, ageBoundary, 0, 8) < 0).ToList();
		}
	}
}
