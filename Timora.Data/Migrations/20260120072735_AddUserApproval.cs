using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timora.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApprovedBy",
                table: "Users",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsApproved",
                table: "Users",
                column: "IsApproved");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_ApprovedBy",
                table: "Users",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ApprovedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ApprovedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IsApproved",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Users");
        }
    }
}
