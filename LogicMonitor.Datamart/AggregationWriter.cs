namespace LogicMonitor.Datamart;

internal static class AggregationWriter
{
	internal static string TableNamePrefix = "DeviceDataSourceInstanceAggregatedData";

	public static string GetTableName(DateTimeOffset start)
		=> $"{TableNamePrefix}_{start.UtcDateTime:yyyyMMdd}";

	/// <summary>
	/// Create a new aggregation table for a new day
	/// </summary>
	internal static async Task<string> EnsureTableExistsAsync(SqlConnection sqlConnection, int sqlCommandTimeoutSeconds, DateTimeOffset start)
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
			command.CommandTimeout = sqlCommandTimeoutSeconds;
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

		using var dbContext = new Context(dbContextOptions);
		var tableName = GetTableName(start);
		logger.LogDebug("Dropping table {tableName}", tableName);
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
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText =
				dbContext.Database.IsSqlServer()
					? $"SELECT name FROM sys.Tables WHERE name LIKE '{TableNamePrefix}%' ORDER BY name"
				: dbContext.Database.IsNpgsql()
					? $"SELECT table_name as name FROM information_schema.tables WHERE table_name LIKE '{TableNamePrefix}%' ORDER BY table_name"
					: throw new NotSupportedException();
			using var reader = await command.ExecuteReaderAsync();
			if (reader.HasRows)
			{
				while (reader.Read())
				{
					tableNames.Add(reader.GetString(0));
				}
			}
		}

		return tableNames;
	}

	internal static async Task WriteAggregations(
		SqlConnection sqlConnection,
		int sqlCommandTimeoutSeconds,
		int sqlBulkCopyTimeoutSeconds,
		int deviceDataSourceInstanceId,
		DateTimeOffset key,
		IEnumerable<DeviceDataSourceInstanceAggregatedDataBulkWriteModel> aggregations,
		ILogger logger)
	{
		var tableName = await EnsureTableExistsAsync(sqlConnection, sqlCommandTimeoutSeconds, key);

		var aggregationCount = aggregations.Count();

		var stopwatch = Stopwatch.StartNew();
		logger.LogTrace(
			"Preparing DataTable for {aggregationCount} aggregations...",
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
			row["PeriodStart"] = aggregation.PeriodStart;
			row["PeriodEnd"] = aggregation.PeriodEnd;
			row["DeviceDataSourceInstanceId"] = aggregation.DeviceDataSourceInstanceId;
			row["DataPointId"] = aggregation.DataPointId;
			row["Min"] = (object)aggregation.Min ?? DBNull.Value;
			row["Max"] = (object)aggregation.Max ?? DBNull.Value;
			row["Sum"] = aggregation.Sum;
			row["SumSquared"] = aggregation.SumSquared;
			row["DataCount"] = aggregation.DataCount;
			row["NoDataCount"] = aggregation.NoDataCount;
			table.Rows.Add(row);
		}

		var lastAggregationHourWrittenUtc = aggregations.Max(a => a.PeriodEnd);
		logger.LogTrace(
			"Preparing DataTable for {aggregationCount} aggregations complete after {stopwatchElapsedMilliseconds:N0}ms.",
			aggregationCount,
			stopwatch.ElapsedMilliseconds);
		stopwatch.Restart();

		using var transaction = sqlConnection.BeginTransaction();
		stopwatch.Restart();
		logger.LogTrace(
			"Bulk writing {aggregationCount} aggregations...",
			aggregationCount);
		// Write out the data as part of a transaction
		using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
		{
			bulkCopy.DestinationTableName = tableName;
			bulkCopy.BulkCopyTimeout = sqlBulkCopyTimeoutSeconds;
			await bulkCopy.WriteToServerAsync(table).ConfigureAwait(false);
		}

		logger.LogTrace(
			"Bulk writing {aggregationCount} aggregations complete after {stopwatchElapsedMilliseconds:N0}ms.",
			aggregationCount,
			stopwatch.ElapsedMilliseconds
			);

		stopwatch.Restart();
		logger.LogTrace(
			"Setting progress for DDSI {deviceDataSourceInstanceId} to {lastAggregationHourWrittenUtc}...",
			deviceDataSourceInstanceId,
			lastAggregationHourWrittenUtc);
		// Update the progress as part of a transaction
		await WriteProgressBoundaryAsync(sqlConnection, sqlCommandTimeoutSeconds, deviceDataSourceInstanceId, lastAggregationHourWrittenUtc, transaction);
		logger.LogTrace(
			"Setting progress for DDSI {deviceDataSourceInstanceId} to {lastAggregationHourWrittenUtc} complete after {stopwatchElapsedMilliseconds:N0}ms.",
			deviceDataSourceInstanceId,
			lastAggregationHourWrittenUtc,
			stopwatch.ElapsedMilliseconds);

		stopwatch.Restart();
		logger.LogTrace("Committing transaction...");
		transaction.Commit();
		logger.LogTrace(
			"Committing transaction complete after {stopwatchElapsedMilliseconds:N0}ms",
			stopwatch.ElapsedMilliseconds);
	}

	internal static async Task WriteProgressBoundaryAsync(SqlConnection sqlConnection, int sqlCommandTimeoutSeconds, int deviceDataSourceInstanceId, DateTime lastAggregationHourWrittenUtc, SqlTransaction transaction)
	{
		const string sql = "update DeviceDataSourceInstances set LastAggregationHourWrittenUtc=@LastAggregationHourWrittenUtc where id=@Id";
		using var command = new SqlCommand(sql, sqlConnection, transaction);
		// Set the CommandTimeout in seconds - it's important this gets written out, it is in a transction, but we wait a bit longer here.
		// Normally this defaults to 30s, but we have observed this timing out on heavily loaded disk based systems.
		command.CommandTimeout = sqlCommandTimeoutSeconds;
		command.Parameters.AddWithValue("@LastAggregationHourWrittenUtc", lastAggregationHourWrittenUtc);
		command.Parameters.AddWithValue("@Id", deviceDataSourceInstanceId);
		await command.ExecuteNonQueryAsync().ConfigureAwait(false);
	}

	internal static async Task PerformAgingAsync(
		DbContextOptions<Context> dbContextOptions,
		int sqlCommandTimeoutSeconds,
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
			await sqlConnection.OpenAsync();
			using var command = new SqlCommand(string.Empty, sqlConnection);
			command.CommandTimeout = sqlCommandTimeoutSeconds;
			foreach (var tableName in tablesToRemove)
			{
				logger.LogInformation("Aging out table {tableName}", tableName);
				command.CommandText = "drop table " + tableName;
				await command.ExecuteNonQueryAsync().ConfigureAwait(false);
			}
		}
	}

	internal static List<string> DetermineTablesToAge(List<string> existingTables, int countAggregationDaysToRetain)
	{
		var ageBoundary = DateTimeOffset.UtcNow.Date.AddDays(-countAggregationDaysToRetain).ToString("yyyyMMdd");
		return existingTables.Where(t => string.CompareOrdinal(t, TableNamePrefix.Length + 1, ageBoundary, 0, 8) < 0).ToList();
	}
}
