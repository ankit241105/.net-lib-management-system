using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lib_management_system.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMembersWithUserBorrowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_Members_MemberId",
                table: "BorrowRecords");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "BorrowRecords",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowRecords_MemberId",
                table: "BorrowRecords",
                newName: "IX_BorrowRecords_UserId");

            migrationBuilder.CreateTable(
                name: "BookRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookRequests_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembershipRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembershipRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookRequests_BookId",
                table: "BookRequests",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookRequests_UserId",
                table: "BookRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRequests_UserId",
                table: "MembershipRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_Users_UserId",
                table: "BorrowRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_Users_UserId",
                table: "BorrowRecords");

            migrationBuilder.DropTable(
                name: "BookRequests");

            migrationBuilder.DropTable(
                name: "MembershipRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BorrowRecords",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowRecords_UserId",
                table: "BorrowRecords",
                newName: "IX_BorrowRecords_MemberId");

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_Members_MemberId",
                table: "BorrowRecords",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
