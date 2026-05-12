using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShadiHall.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6814), new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6823) });

            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6835), new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6835) });

            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6838), new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6838) });

            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6841), new DateTime(2026, 5, 9, 17, 47, 20, 975, DateTimeKind.Utc).AddTicks(6842) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bdbbd0c5-7048-45c5-9d99-3b29e80a8566", new DateTime(2026, 5, 9, 17, 47, 20, 926, DateTimeKind.Utc).AddTicks(2399), "AQAAAAIAAYagAAAAEKBRXF8lSDwUubT8yKD5s8XnNx73bJ3Itg0KtiAjs9vTFQAfwDR1VFfGeIAmq8Y96g==", "1201e58c-f5eb-45ef-870c-c13d0f835a85" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9781), new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9789) });

            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9802), new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9802) });

            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9805), new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9805) });

            migrationBuilder.UpdateData(
                table: "Halls",
                keyColumn: "HallId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9807), new DateTime(2026, 5, 9, 17, 29, 9, 727, DateTimeKind.Utc).AddTicks(9807) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0b9bce60-9b69-41c4-81d2-e761e4ee3b74", new DateTime(2026, 5, 9, 17, 29, 9, 682, DateTimeKind.Utc).AddTicks(859), "AQAAAAIAAYagAAAAEBovqIOhX0QNlXeEkBehfS40GbcLX5MiJluAuZ9zlV5VpuweTIndD0/5IlF9FdGc2w==", "4c8f71a9-7527-4502-81e8-5fbd57c75ce8" });
        }
    }
}
