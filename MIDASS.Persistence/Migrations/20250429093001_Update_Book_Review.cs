using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Book_Review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BookReviews",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateReview",
                table: "BookReviews",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateReview",
                table: "BookReviews");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BookReviews",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
