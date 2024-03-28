using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ratio_Lyrics.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class addcolumnisclient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClientUser",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClientUser",
                table: "AspNetUsers");
        }
    }
}
