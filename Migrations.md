# Migrations

Migrations are used to update the database schema. They are created using the Entity Framework Core CLI.
Separate migrations are created for the PostgreSQL and SQL Server databases.

When adding migrations, you should keep all migration sets in sync, using:

```powershell
Read-Host "Enter the migration name" | ForEach-Object {
	dotnet ef migrations add $_ `
		--project "LogicMonitor.Datamart" `
		--context NpgsqlContext `
		--output-dir Migrations/NpgsqlMigrations
	dotnet ef migrations add $_ `
		--project "LogicMonitor.Datamart" `
		--context SqlServerContext `
		--output-dir Migrations/SqlServerMigrations
}
```

You must have the EF Core CLI Tool installed.

If not installed and the migration complains, you can can install it using:

```powershell
dotnet tool install --global dotnet-ef

# or update it using:
dotnet tool update --global dotnet-ef
```
After installation, veryify the installation using:
```powershell
dotnet ef --version
```