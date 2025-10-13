using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timora.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureFirebaseIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirebaseId",
                table: "Users",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirebaseId",
                table: "Users",
                column: "FirebaseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_FirebaseId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirebaseId",
                table: "Users");
        }
    }
}
