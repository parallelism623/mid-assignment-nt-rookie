using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASM.Persistence.Migrations.AuditLogDb
{
    /// <inheritdoc />
    public partial class InitialDatabasea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "PropertyTypeFullName",
                table: "AuditLogDatas");

            migrationBuilder.RenameColumn(
                name: "SuccessDescription",
                table: "AuditLogs",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "ErrorDescription",
                table: "AuditLogs",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "AuditLogs",
                newName: "SuccessDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AuditLogs",
                newName: "ErrorDescription");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PropertyTypeFullName",
                table: "AuditLogDatas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
