using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExtend",
                table: "BookBorrowingRequestDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExtend",
                table: "BookBorrowingRequestDetails");
        }
    }
}
