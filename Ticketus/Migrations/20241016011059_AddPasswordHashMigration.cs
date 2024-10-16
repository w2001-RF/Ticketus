using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ticketus.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "TicketId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "DateJoined", "Email", "UserName" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 15, 19, 10, 37, 160, DateTimeKind.Local).AddTicks(3905), "john.doe@example.com", "JohnDoe" },
                    { 2, new DateTime(2024, 10, 15, 19, 10, 37, 160, DateTimeKind.Local).AddTicks(3914), "jane.smith@example.com", "JaneSmith" }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "TicketId", "DateCreated", "Description", "StatusId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 15, 19, 10, 37, 160, DateTimeKind.Local).AddTicks(3957), "Fix homepage bug", 1, 1 },
                    { 2, new DateTime(2024, 10, 15, 19, 10, 37, 160, DateTimeKind.Local).AddTicks(3966), "Update contact page", 2, 2 }
                });
        }
    }
}
