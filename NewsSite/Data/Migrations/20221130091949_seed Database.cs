using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewsSite.Migrations
{
    /// <inheritdoc />
    public partial class seedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1e5d9146-8041-4974-ab63-bd95c3f7c8ef", "1e5d9146-8041-4974-ab63-bd95c3f7c8ef", "Admin", "Admin" },
                    { "a97b8250-fd63-4186-a595-67f8743d15fc", "a97b8250-fd63-4186-a595-67f8743d15fc", "Visitor", "VISITOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "City", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PostalCode", "SecurityStamp", "State", "StreetAddress", "TwoFactorEnabled", "UserName" },
                values: new object[] { "02174cf0–9412–4cfe - afbf - 59f706d72cf6", 0, null, "0bb224a8-551a-4a79-b2c6-7dc6179ef76a", "ApplicationUser", "admin@gmail.com", true, false, null, "Admin", null, "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAENQzggaN/5mftc31bt+hixbaW2w3WpCrS/4BMJ6PSqs/8RlM/+8h1OrRHXFLbaASpg==", null, false, null, "bcac861a-d464-49fc-8492-d1652050ea47", null, null, false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1e5d9146-8041-4974-ab63-bd95c3f7c8ef", "02174cf0–9412–4cfe - afbf - 59f706d72cf6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a97b8250-fd63-4186-a595-67f8743d15fc");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1e5d9146-8041-4974-ab63-bd95c3f7c8ef", "02174cf0–9412–4cfe - afbf - 59f706d72cf6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e5d9146-8041-4974-ab63-bd95c3f7c8ef");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe - afbf - 59f706d72cf6");
        }
    }
}
