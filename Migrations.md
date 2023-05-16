# Migrations

To create a migration, use:

```powershell
Read-Host "Enter the migration name" | ForEach-Object { dotnet ef migrations add $_ --project "LogicMonitor.Datamart" }

OR 

dotnet ef migrations add "InitialMigration" --project "LogicMonitor.Datamart"
```