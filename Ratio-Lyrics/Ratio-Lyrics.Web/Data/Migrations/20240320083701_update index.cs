using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ratio_Lyrics.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_SearchKey",
                table: "Songs");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Songs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_DisplayName_SearchKey",
                table: "Songs",
                columns: new[] { "DisplayName", "SearchKey" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_DisplayName_SearchKey",
                table: "Songs");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_SearchKey",
                table: "Songs",
                column: "SearchKey");
        }
    }
}
