using LogicMonitor.Datamart.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
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
		internal static async Task DropTableAsync(DbContextOptions<Context> dbContextOptions, DateTimeOffset start)
		{
			if (dbContextOptions == null)
			{
				throw new ArgumentNullException(nameof(dbContextOptions));
			}

			using (var dbContext = new Context(dbContextOptions))
			{
				var tableName = GetTableName(start);
				var tableCreationSql = "DROP TABLE [" + tableName + "]";
				await dbContext.Database.ExecuteSqlRawAsync(tableCreationSql).ConfigureAwait(false);
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

			using (var transaction = sqlConnection.BeginTransaction())
			{
				// Write out the data as part of a transaction
				using (var bulk = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
				{
					bulk.DestinationTableName = tableName;
					await bulk.WriteToServerAsync(table).ConfigureAwait(false);
				}

				// Update the progress as part of a transaction
				await WriteProgressBoundaryAsync(sqlConnection, deviceDataSourceInstanceId, lastAggregationHourWrittenUtc, transaction);

				await transaction.CommitAsync().ConfigureAwait(false);
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
	}
}
