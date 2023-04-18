using System.Globalization;

namespace LogicMonitor.Datamart;

internal static class AggregationWriter
{
	internal static string TableNamePrefix = "DeviceDataSourceInstanceAggregatedData";
	internal static int SqlTimeoutSeconds = 100;

	public static string GetTableName(DateTimeOffset start)
		=> $"{TableNamePrefix}_{start.UtcDateTime:yyyyMMdd}";

	/// <summary>
	/// Create a new aggregation table for a new day
	/// </summary>
	internal static async Task<string> EnsureTableExistsAsync(
		SqlConnection sqlConnection,
		DateTimeOffset start)
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
			command.CommandTimeout = SqlTimeoutSeconds;
			await command
				.ExecuteNonQueryAsync()
				.ConfigureAwait(false);
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

		using var dbContext = new Context(dbContextOptions);
		var tableName = GetTableName(start);
		logger.LogDebug("Dropping table {TableName}", tableName);
		var tableCreationSql = "DROP TABLE [" + tableName + "]";
		await dbContext
			.Database
			.ExecuteSqlRawAsync(tableCreationSql)
			.ConfigureAwait(false);
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
			using var connection = dbContext.Database.GetDbConnection();
			await connection
				.OpenAsync()
				.ConfigureAwait(false);
			using var command = connection.CreateCommand();
			command.CommandText =
				dbContext.Database.IsSqlServer()
					? $"SELECT name FROM sys.Tables WHERE name LIKE '{TableNamePrefix}%' ORDER BY name"
				: dbContext.Database.IsNpgsql()
					? $"SELECT table_name as name FROM information_schema.tables WHERE table_name LIKE '{TableNamePrefix}%' ORDER BY table_name"
					: throw new NotSupportedException();
			using var reader = await command
				.ExecuteReaderAsync()
				.ConfigureAwait(false);
			if (reader.HasRows)
			{
				while (await reader.ReadAsync().ConfigureAwait(false))
				{
					tableNames.Add(reader.GetString(0));
				}
			}
		}

		return tableNames;
	}

	internal static async Task WriteAggregations(
		SqlConnection sqlConnection,
		IEnumerable<TimeSeriesDataAggregationStoreItem> aggregations,
		int deviceDataSourceInstanceId,
		DateTimeOffset key,
		ILogger logger)
	{
		var tableName = await EnsureTableExistsAsync(sqlConnection, key)
			.ConfigureAwait(false);

		var aggregationCount = aggregations.Count();

		var stopwatch = Stopwatch.StartNew();
		logger.LogTrace(
			"Preparing DataTable for {AggregationCount} aggregations...",
			aggregationCount);

		// Prep the data into a DataTable, setting initial structure from the database
		using var table = new DataTable();
		using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM {tableName}", sqlConnection))
		{
			adapter.Fill(table);
		}

		// Populate the DataTable
		foreach (var aggregation in aggregations)
		{
			var row = table.NewRow();
			row["Id"] = Guid.NewGuid();
			row["DataPointId"] = aggregation.DataPointId;
			row["PeriodStart"] = aggregation.PeriodStart;
			row["PeriodEnd"] = aggregation.PeriodEnd;
			row["Centile05"] = aggregation.Centile05.HasValue ? aggregation.Centile05.Value : DBNull.Value;
			row["Centile10"] = aggregation.Centile10.HasValue ? aggregation.Centile10.Value : DBNull.Value;
			row["Centile25"] = aggregation.Centile25.HasValue ? aggregation.Centile25.Value : DBNull.Value;
			row["Centile75"] = aggregation.Centile75.HasValue ? aggregation.Centile75.Value : DBNull.Value;
			row["Centile90"] = aggregation.Centile90.HasValue ? aggregation.Centile90.Value : DBNull.Value;
			row["Centile95"] = aggregation.Centile95.HasValue ? aggregation.Centile95.Value : DBNull.Value;
			row["First"] = aggregation.First.HasValue ? aggregation.First.Value : DBNull.Value;
			row["Last"] = aggregation.Last.HasValue ? aggregation.Last.Value : DBNull.Value;
			row["FirstWithData"] = aggregation.FirstWithData.HasValue ? aggregation.FirstWithData.Value : DBNull.Value;
			row["LastWithData"] = aggregation.LastWithData.HasValue ? aggregation.LastWithData.Value : DBNull.Value;
			row["Min"] = aggregation.Min.HasValue ? aggregation.Min.Value : DBNull.Value;
			row["Max"] = aggregation.Max.HasValue ? aggregation.Max.Value : DBNull.Value;
			row["Sum"] = aggregation.Sum;
			row["SumSquared"] = aggregation.SumSquared;
			row["DataCount"] = aggregation.DataCount;
			row["NoDataCount"] = aggregation.NoDataCount;
			row["NormalCount"] = aggregation.NormalCount;
			row["WarningCount"] = aggregation.WarningCount;
			row["ErrorCount"] = aggregation.ErrorCount;
			row["CriticalCount"] = aggregation.CriticalCount;
			table.Rows.Add(row);
		}

		var lastAggregationHourWrittenUtc = aggregations.Max(a => a.PeriodEnd);
		logger.LogTrace(
			"Preparing DataTable for {AggregationCount} aggregations complete after {StopwatchElapsedMilliseconds:N0}ms.",
			aggregationCount,
			stopwatch.ElapsedMilliseconds);
		stopwatch.Restart();

		using var transaction = sqlConnection.BeginTransaction();
		stopwatch.Restart();
		logger.LogTrace(
			"Bulk writing {AggregationCount} aggregations...",
			aggregationCount);
		// Write out the data as part of a transaction
		using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
		{
			bulkCopy.DestinationTableName = tableName;
			bulkCopy.BulkCopyTimeout = SqlTimeoutSeconds;
			await bulkCopy.WriteToServerAsync(table).ConfigureAwait(false);
		}

		logger.LogTrace(
			"Bulk writing {AggregationCount} aggregations complete after {StopwatchElapsedMilliseconds:N0}ms.",
			aggregationCount,
			stopwatch.ElapsedMilliseconds
			);

		stopwatch.Restart();
		logger.LogTrace(
			"Setting progress for DDSI {DeviceDataSourceInstanceId} to {LastAggregationHourWrittenUtc}...",
			deviceDataSourceInstanceId,
			lastAggregationHourWrittenUtc);
		// Update the progress as part of a transaction
		await WriteProgressBoundaryAsync(
			sqlConnection,
			SqlTimeoutSeconds,
			deviceDataSourceInstanceId,
			lastAggregationHourWrittenUtc,
			transaction
		)
			.ConfigureAwait(false);
		logger.LogTrace(
			"Setting progress for DDSI {DeviceDataSourceInstanceId} to {LastAggregationHourWrittenUtc} complete after {StopwatchElapsedMilliseconds:N0}ms.",
			deviceDataSourceInstanceId,
			lastAggregationHourWrittenUtc,
			stopwatch.ElapsedMilliseconds);

		stopwatch.Restart();
		logger.LogTrace("Committing transaction...");
		transaction.Commit();
		logger.LogTrace(
			"Committing transaction complete after {StopwatchElapsedMilliseconds:N0}ms",
			stopwatch.ElapsedMilliseconds);
	}

	internal static async Task WriteProgressBoundaryAsync(
		SqlConnection sqlConnection,
		int sqlCommandTimeoutSeconds,
		int deviceDataSourceInstanceId,
		DateTimeOffset lastAggregationHourWrittenUtc,
		SqlTransaction transaction)
	{
		const string sql = "update DeviceDataSourceInstances set LastAggregationHourWrittenUtc=@LastAggregationHourWrittenUtc where id=@Id";
		using var command = new SqlCommand(sql, sqlConnection, transaction);
		// Set the CommandTimeout in seconds - it's important this gets written out, it is in a transaction, but we wait a bit longer here.
		// Normally this defaults to 30s, but we have observed this timing out on heavily loaded disk based systems.
		command.CommandTimeout = sqlCommandTimeoutSeconds;
		command.Parameters.AddWithValue("@LastAggregationHourWrittenUtc", lastAggregationHourWrittenUtc);
		command.Parameters.AddWithValue("@Id", deviceDataSourceInstanceId);
		await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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
			using var context = new Context(dbContextOptions);
			using var dbConnection = context.Database.GetDbConnection();
			using var sqlConnection = new SqlConnection(dbConnection.ConnectionString);
			await sqlConnection
				.OpenAsync()
				.ConfigureAwait(false);
			using var command = new SqlCommand(string.Empty, sqlConnection);
			command.CommandTimeout = SqlTimeoutSeconds;
			foreach (var tableName in tablesToRemove)
			{
				logger.LogInformation("Aging out table {TableName}", tableName);
				command.CommandText = "drop table " + tableName;
				await command.ExecuteNonQueryAsync().ConfigureAwait(false);
			}
		}
	}

	internal static List<string> DetermineTablesToAge(List<string> existingTables, int countAggregationDaysToRetain)
	{
		var ageBoundary = DateTimeOffset
			.UtcNow
			.Date
			.AddDays(-countAggregationDaysToRetain)
			.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
		return existingTables.Where(t => string.CompareOrdinal(t, TableNamePrefix.Length + 1, ageBoundary, 0, 8) < 0).ToList();
	}
}
