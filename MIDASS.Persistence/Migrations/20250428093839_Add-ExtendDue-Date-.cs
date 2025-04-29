using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendDueDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ExtendDueDate",
                table: "BookBorrowingRequestDetails",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtendDueDate",
                table: "BookBorrowingRequestDetails");
        }
    }
}
