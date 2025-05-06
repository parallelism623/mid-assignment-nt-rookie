using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrowingRequests_Users_UserId",
                table: "BookBorrowingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Requester",
                table: "BookBorrowingRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BookBorrowingRequests",
                newName: "ApproverId");

            migrationBuilder.RenameIndex(
                name: "IX_BookBorrowingRequests_UserId",
                table: "BookBorrowingRequests",
                newName: "IX_BookBorrowingRequests_ApproverId");

            migrationBuilder.AddColumn<byte[]>(
                name: "TimeStamp",
                table: "Books",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateRequested",
                table: "BookBorrowingRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "RequesterId",
                table: "BookBorrowingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrowingRequests_RequesterId",
                table: "BookBorrowingRequests",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrowingRequests_Users_ApproverId",
                table: "BookBorrowingRequests",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrowingRequests_Users_RequesterId",
                table: "BookBorrowingRequests",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrowingRequests_Users_ApproverId",
                table: "BookBorrowingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrowingRequests_Users_RequesterId",
                table: "BookBorrowingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_BookBorrowingRequests_RequesterId",
                table: "BookBorrowingRequests");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DateRequested",
                table: "BookBorrowingRequests");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "BookBorrowingRequests");

            migrationBuilder.RenameColumn(
                name: "ApproverId",
                table: "BookBorrowingRequests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookBorrowingRequests_ApproverId",
                table: "BookBorrowingRequests",
                newName: "IX_BookBorrowingRequests_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Requester",
                table: "BookBorrowingRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrowingRequests_Users_UserId",
                table: "BookBorrowingRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
