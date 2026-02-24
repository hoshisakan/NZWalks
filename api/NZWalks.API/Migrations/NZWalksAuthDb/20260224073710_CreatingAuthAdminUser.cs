using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NZWalks.API.Migrations.NZWalksAuthDb
{
    /// <inheritdoc />
    public partial class CreatingAuthAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "89009c2e-ca38-4ea2-86d2-1db2d5f30449", "89009c2e-ca38-4ea2-86d2-1db2d5f30449", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4c791d74-5907-40ce-b188-034e6d60fed9", 0, "5dc2f2e7-93ca-4b89-8c56-4811084fb33c", "admin@example.com", false, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEDi0U/fHD9MZtmjKy4TrPwnSN/H9G2Uk9jmgeDFU88/390WMauiTYvwpCb/OcrFgZg==", null, false, "9a8e4b72-71ed-47d3-9ee6-df9b89c8b294", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "89009c2e-ca38-4ea2-86d2-1db2d5f30449", "4c791d74-5907-40ce-b188-034e6d60fed9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "89009c2e-ca38-4ea2-86d2-1db2d5f30449", "4c791d74-5907-40ce-b188-034e6d60fed9" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89009c2e-ca38-4ea2-86d2-1db2d5f30449");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4c791d74-5907-40ce-b188-034e6d60fed9");
        }
    }
}
