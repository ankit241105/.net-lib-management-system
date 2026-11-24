using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lib_management_system.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEmailAndReminderFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderSentAt",
                table: "BorrowRecords",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReminderSentAt",
                table: "BorrowRecords");
        }
    }
}
