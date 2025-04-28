using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIDASS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDatabaseOneMore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerifyCode",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerifyCode",
                table: "Users");
        }
    }
}
