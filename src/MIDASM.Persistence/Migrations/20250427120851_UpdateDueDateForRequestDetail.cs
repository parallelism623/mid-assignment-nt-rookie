using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDueDateForRequestDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtendDueDateTimes",
                table: "BookBorrowingRequestDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtendDueDateTimes",
                table: "BookBorrowingRequestDetails");
        }
    }
}
