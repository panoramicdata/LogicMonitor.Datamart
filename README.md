# Introduction

## Purpose

The LogicMonitor Datamart creates a copy of major aspects of your LogicMonitor system in a SQL Server or PostgreSQL database, including:
- Dimension data
- Alert history
- Audit log history
- Time Series data aggregations

## Support

* Customers with an active project and available hours can request support via your project manager.
* Otherwise, support is provided on a best-effort basis via the [Panoramic Data Community](https://community.panoramicdata.com/).
* The LogicMonitor Datamart is provided as-is, with no warranty or guarantee of support.

# Installation

## Prerequisites

1. **LogicMonitor Account**
	- You will need an API key and ID to access the LogicMonitor API.
	- Create these in the LogicMonitor UI under Settings > Users > API Tokens.
	- We recommend creating a separate user for this purpose.
	- Full read permissions, including LogicModule access, are required.

2. **Database Server**
	- Supported databases: PostgreSQL and SQL Server.
	- Note: Table partitioning is not available in lower SQL Server tiers, such as SQL Server Express.
	- The database server must be accessible from the container.

3. **Docker Environment**
	- Step-by-step instructions are provided for Docker Desktop on Windows.
	- Basic understanding of Docker and container management is assumed.

4. **Configuration File**
	- An example configuration file is provided here: [appsettings.example.json](LogicMonitor.Datamart.Cli/appsettings.example.json).
	- Modify this file to include your API credentials, database connection information, and data mart configuration.
	- Ensure the configuration file is accessible from the container.

## Configuration file

* **Name**
	- This is just a name for the configuration. Optional.

### LogicMonitor configuration

* * **DataSources**
	- A list of data sources for which time series data will be collected.
	- Each data source should have the following specified:
		- **Name**
			- The UNIQUE name of the data source in LogicMonitor.
		- **InstanceInclusionExpression**
			- An [Extended NCalc expression](https://github.com/panoramicdata/PanoramicData.NCalcExtensions).  If that scares you, just leave it as the default string: "true".
		- **DataPoints**: A list of data points to collect for this data source.
			- **Name**
				- The name of the data point in LogicMonitor from the "Raw Data" view.
			- **Description**
				- A description of the data point.
			- **GraphName**
				- The name of the graph in LogicMonitor.
				- Leave as null.
			- **MeasurementUnit**
				- The unit of measurement for the data point.
				- LogicMonitor don't provide this in-product, so you'll need to work it out by reading the DataSource's source code.
			- **PercentageAvailabilityCalculation**
				- Another NCalc.
				- Leave it as "" or contact us for support.
			- **GlobalAlertExpression**
				- Use this to override LogicMonitor's alerting logic.
				- Leave as "" or contact us for support.
			- **Calculation**
				- Another NCalc.
				- Leave it as "" or contact us for support.
			- **Tags**
				- The tags to apply to the data point.
				- These are directly entered into the database, so you can use any string you like.
			- **Property1, Property2...**
				- Up to 20 available.
				- These are copied from the LogicMonitor Resource's properties if available.
			- **InstanceDataPointProperty1, Property2...**
				- Up to 20 available.
				- These are copied from the LogicMonitor ResourceDataSourceInstance's properties if available.
			- **ResyncTimeSeriesData**
				- If true, the time series data for this data point will be re-collected.
				- This is useful if you've changed the data point's configuration in LogicMonitor.
* **AggregationReset**
	- If you're resetting all aggregations, set this to true.
	- This will delete all existing aggregations for this DataPoint.
	- If you're NOT resetting aggregations, set this to false.
* **StartTimeUtc**
	- This is the time from which time series data will be collected.
	- Must be midnight on a month boundary, e.g. 2024-01-01
	- Should be set the first time you run the data mart.
	- Be cautious not to set this too far back, as it will collect a lot of data.
	- Depending on the amount of data, the first run could take up to a week, so be prepared for that!
	- You cannot backfill data before existing time-series data.
* **LogicMonitorClientOptions**
	- **Account**
		- The name of your LogicMonitor account.
	- **AccessId**
		- The access ID of your LogicMonitor API user.
	- **AccessKey**
		- The access key of your LogicMonitor API user.

### Database configuration

* **DatabaseType**
	- The type of database you are using.
	- Options are "SqlServer" or "Postgres".
* **DatabaseServerName**
	- The IP address or DNS name of the database server.
	- If connecting to a database running on the container host, you should use the special hostname `host.docker.internal`, and not 'localhost' or '127.0.0.1'.
* **DatabaseServerPort**
	- The port on which the database server is listening.
	- Default is 1433 for SQL Server and 5432 for PostgreSQL.
* **DatabaseRetryOnFailureCount**
	- The number of times to retry a failed database operation.
	- Default is 0.
* **SqlServerAuthenticationMethod**
	- The authentication method to use when connecting to SQL Server.
	- Options are documented in the [Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlauthenticationmethod).
* **DatabaseName**
	- The name of the database to use.
	- We recommned creating a new database for the data mart and ensuring that your LogicMonitor portal name is included in the database name.
	- For example, if your LogicMonitor portal is `acme`, you could name the database `LogicMonitor_acme`.
	- This is important if you are running multiple data marts for different portals.
* **DatabaseUsername**
	- The username to use when connecting to the database.
* **DatabasePassword**
	- The password to use when connecting to the database.
* **SqlCommandTimeoutSeconds**
	- The number of seconds to wait before a SQL command times out.
	- Default is 600.
* **SqlBulkCopyTimeoutSeconds**
	- The number of seconds to wait before a SQL bulk copy operation times out.
	- Default is 600.
* **EnableSensitiveDatabaseLogging**
	- If true, sensitive database information will be logged.
	- Default is false.
* **DeviceDataSourceInstanceBatchSize**
	- The number of DeviceDataSourceInstances to process in a batch.
	- Default is 100.
* **DeviceProperties**
	- A list of properties to collect from the LogicMonitor API.
	- These are copied from the LogicMonitor Resource's properties.
	- Up to 20 properties can be collected.
* **DimensionSyncHaltOnError**
	- If true, the dimension sync will halt on error.
	- Default is true.
* **MinutesOffset**
	- The number of minutes to offset the time series data.
	- Default is 0.
* **FakeExecutionTime**
	- If set, the data mart will use this time instead of the current time.
	- This is useful for testing.
	- Default is null.

# Docker container

## Using a Local `appsettings.json` File with Docker

To use the Docker image `panoramicdata/logicmonitor-datamart:latest` with a local `appsettings.json` file, follow these steps:

## Steps:

1. **Prepare your local `appsettings.json` file**:
   Ensure you have a local `appsettings.json` file, which the container will use.  See above for an example configuration file.

   For example, let's assume the file is located at `/path/to/your/appsettings.json`.

2. **Run the Docker container with a volume mount**:
   Use Docker's `-v` flag to mount your local file into the container. Set the environment variable `CONFIG_FILE` to point to the mounted file inside the container.

   Example command:

 ```
   docker run -d \
   -v /path/to/your/appsettings.json:/app/appsettings.json \
   -e CONFIG_FILE=/app/appsettings.json \
   panoramicdata/logicmonitor-datamart:latest
```

   - `-v /path/to/your/appsettings.json:/app/appsettings.json`: This mounts your local `appsettings.json` file into the container at `/app/appsettings.json`.
   - `-e CONFIG_FILE=/app/appsettings.json`: This sets the `CONFIG_FILE` environment variable to tell the container where the configuration file is located.

3. **Explanation**:
   - Your local `appsettings.json` file is mounted inside the container at `/app/appsettings.json`.
   - The application inside the container reads the configuration from the file specified by the `CONFIG_FILE` environment variable.

4. **Verify the container**:
   You can verify that the container is running and using the correct configuration file by checking the logs:

```
   docker logs <container-id>
```

   Replace `<container-id>` with the ID of your running container (which you can get by running `docker ps`).


powershell

# For developers

Developers can use the LogicMonitor.Datamart nuget package to create their own data mart.
The package is available on nuget.org and is licensed under the MIT license.

# Contributors

Contributions are welcome.
