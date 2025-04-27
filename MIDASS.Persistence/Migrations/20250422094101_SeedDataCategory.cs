using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MIDASS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("0a1b2c3d-4e5f-6a78-789a-bcdef0123456"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Destination guides, travel tips, and itinerary ideas.", false, null, null, "Travel" },
                    { new Guid("1b2c3d4e-5f6a-7b89-89ab-cdef01234567"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Recipes, restaurant reviews, and culinary trends.", false, null, null, "Food" },
                    { new Guid("2c3d4e5f-6a7b-8c90-9abc-def012345678"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Market analysis, startup tips, and corporate news.", false, null, null, "Business" },
                    { new Guid("3d4e5f6a-7b8c-9d01-abcd-ef0123456789"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), null, false, null, null, "Beauty" },
                    { new Guid("a3f1c2d4-5b6e-7f80-1234-56789abcdef0"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Latest news, tutorials, and updates about technology.", false, null, null, "Technology" },
                    { new Guid("b4e2d3c5-6f7a-8b90-2345-6789abcdef01"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Discoveries, research articles, and scientific insights.", false, null, null, "Science" },
                    { new Guid("c5d3e4b6-7a8b-9c01-3456-789abcdef012"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Latest scores, analyses, and commentary on sports events.", false, null, null, "Sports" },
                    { new Guid("d6e4f5c7-8b9c-0d12-4567-89abcdef0123"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Movie reviews, music releases, and celebrity news.", false, null, null, "Entertainment" },
                    { new Guid("e7f5a6d8-9c0d-1e23-5678-9abcdef01234"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Study tips, course recommendations, and learning resources.", false, null, null, "Education" },
                    { new Guid("f8a6b7e9-0d1e-2f34-6789-abcdef012345"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Wellness advice, medical news, and healthy living guides.", false, null, null, "Health" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("0a1b2c3d-4e5f-6a78-789a-bcdef0123456"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1b2c3d4e-5f6a-7b89-89ab-cdef01234567"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("2c3d4e5f-6a7b-8c90-9abc-def012345678"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("3d4e5f6a-7b8c-9d01-abcd-ef0123456789"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a3f1c2d4-5b6e-7f80-1234-56789abcdef0"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b4e2d3c5-6f7a-8b90-2345-6789abcdef01"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c5d3e4b6-7a8b-9c01-3456-789abcdef012"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d6e4f5c7-8b9c-0d12-4567-89abcdef0123"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("e7f5a6d8-9c0d-1e23-5678-9abcdef01234"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f8a6b7e9-0d1e-2f34-6789-abcdef012345"));
        }
    }
}
