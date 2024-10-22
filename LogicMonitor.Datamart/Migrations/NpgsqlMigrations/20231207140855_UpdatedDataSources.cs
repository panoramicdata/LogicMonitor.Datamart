#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class UpdatedDataSources : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Checksum",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "CollectionAttributeIp",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "CollectionAttributeName",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Group",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataAuditedRegistryId",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataAuditedVersion",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "InstallationMetadataIsChangedFromOrigin",
				table: "DataSources",
				type: "boolean",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "InstallationMetadataIsChangedFromTargetLastPublished",
				table: "DataSources",
				type: "boolean",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "InstallationMetadataLogicModuleId",
				table: "DataSources",
				type: "integer",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataLogicModuleType",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataOriginAuthorCompanyUuid",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataOriginAuthorNamespace",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataOriginChecksum",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataOriginLineageId",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataOriginRegistryId",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataOriginVersion",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataTargetLastPublishedChecksum",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataTargetLastPublishedId",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataTargetLastPublishedVersion",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "InstallationMetadataTargetLineageId",
				table: "DataSources",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "LineageId",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "PayloadVersion",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Tags",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<bool>(
				name: "UseWildValueAsUuid",
				table: "DataSources",
				type: "boolean",
				nullable: false,
				defaultValue: false);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Checksum",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "CollectionAttributeIp",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "CollectionAttributeName",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "Group",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataAuditedRegistryId",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataAuditedVersion",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataIsChangedFromOrigin",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataIsChangedFromTargetLastPublished",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataLogicModuleId",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataLogicModuleType",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataOriginAuthorCompanyUuid",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataOriginAuthorNamespace",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataOriginChecksum",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataOriginLineageId",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataOriginRegistryId",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataOriginVersion",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataTargetLastPublishedChecksum",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataTargetLastPublishedId",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataTargetLastPublishedVersion",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "InstallationMetadataTargetLineageId",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "LineageId",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "PayloadVersion",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "Tags",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "UseWildValueAsUuid",
				table: "DataSources");
		}
	}
}
