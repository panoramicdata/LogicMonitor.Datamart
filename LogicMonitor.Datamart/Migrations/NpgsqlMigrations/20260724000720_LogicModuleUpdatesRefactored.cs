using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
    /// <inheritdoc />
    public partial class LogicModuleUpdatesRefactored : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliesTo",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "AuditVersion",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "CurrentUuid",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Local",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "LocalId",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Namespace",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Quality",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "RegistryVersion",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Remote",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "RestLm",
                table: "LogicModuleUpdates");

            migrationBuilder.RenameColumn(
                name: "PublishedAtMilliseconds",
                table: "LogicModuleUpdates",
                newName: "UpdatedAtMs");

            migrationBuilder.RenameColumn(
                name: "LocalVersion",
                table: "LogicModuleUpdates",
                newName: "OriginPublishedAtMs");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "LogicModuleUpdates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Locator",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Group",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CollectionMethod",
                table: "LogicModuleUpdates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "AuthorPortalName",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExchangeId",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HasUpdateAvailable",
                table: "LogicModuleUpdates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsChangedFromTargetLastPublished",
                table: "LogicModuleUpdates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomized",
                table: "LogicModuleUpdates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeprecated",
                table: "LogicModuleUpdates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInUse",
                table: "LogicModuleUpdates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInstalled",
                table: "LogicModuleUpdates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginAuthorNamespace",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginLocator",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginName",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginRegistryId",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginStatus",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginVersion",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpgradeableRegistryId",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AppliesTo",
                table: "EventSources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditVersion",
                table: "EventSources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Checksum",
                table: "EventSources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "EventSources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsInUse",
                table: "EventSources",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "EventSources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsInUse",
                table: "DataSources",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInUse",
                table: "ConfigSources",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorPortalName",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "ExchangeId",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "HasUpdateAvailable",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "IsChangedFromTargetLastPublished",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "IsCustomized",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "IsDeprecated",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "IsInUse",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "IsInstalled",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "OriginAuthorNamespace",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "OriginLocator",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "OriginName",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "OriginRegistryId",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "OriginStatus",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "OriginVersion",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "UpgradeableRegistryId",
                table: "LogicModuleUpdates");

            migrationBuilder.DropColumn(
                name: "AppliesTo",
                table: "EventSources");

            migrationBuilder.DropColumn(
                name: "AuditVersion",
                table: "EventSources");

            migrationBuilder.DropColumn(
                name: "Checksum",
                table: "EventSources");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "EventSources");

            migrationBuilder.DropColumn(
                name: "IsInUse",
                table: "EventSources");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "EventSources");

            migrationBuilder.DropColumn(
                name: "IsInUse",
                table: "DataSources");

            migrationBuilder.DropColumn(
                name: "IsInUse",
                table: "ConfigSources");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtMs",
                table: "LogicModuleUpdates",
                newName: "PublishedAtMilliseconds");

            migrationBuilder.RenameColumn(
                name: "OriginPublishedAtMs",
                table: "LogicModuleUpdates",
                newName: "LocalVersion");

            migrationBuilder.AlterColumn<long>(
                name: "Version",
                table: "LogicModuleUpdates",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "LogicModuleUpdates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Locator",
                table: "LogicModuleUpdates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Group",
                table: "LogicModuleUpdates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CollectionMethod",
                table: "LogicModuleUpdates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "AppliesTo",
                table: "LogicModuleUpdates",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "AuditVersion",
                table: "LogicModuleUpdates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "LogicModuleUpdates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentUuid",
                table: "LogicModuleUpdates",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Local",
                table: "LogicModuleUpdates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LocalId",
                table: "LogicModuleUpdates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Namespace",
                table: "LogicModuleUpdates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Quality",
                table: "LogicModuleUpdates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistryVersion",
                table: "LogicModuleUpdates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Remote",
                table: "LogicModuleUpdates",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RestLm",
                table: "LogicModuleUpdates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
