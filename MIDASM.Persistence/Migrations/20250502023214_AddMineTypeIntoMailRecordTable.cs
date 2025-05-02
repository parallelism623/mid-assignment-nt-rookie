using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMineTypeIntoMailRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MineType",
                table: "EmailRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MineType",
                table: "EmailRecords");
        }
    }
}
