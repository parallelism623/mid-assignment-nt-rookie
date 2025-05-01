using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChaneCori : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrowingRequestDetails_Books_BookBorrowingRequestId",
                table: "BookBorrowingRequestDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrowingRequestDetails_Books_BookId",
                table: "BookBorrowingRequestDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrowingRequestDetails_Books_BookId",
                table: "BookBorrowingRequestDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrowingRequestDetails_Books_BookBorrowingRequestId",
                table: "BookBorrowingRequestDetails",
                column: "BookBorrowingRequestId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
