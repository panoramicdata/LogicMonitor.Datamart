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
		ArgumentNullException.ThrowIfNull(sqlConnection);

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
		ArgumentNullException.ThrowIfNull(dbContextOptions);

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
		ArgumentNullException.ThrowIfNull(dbContextOptions);

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
