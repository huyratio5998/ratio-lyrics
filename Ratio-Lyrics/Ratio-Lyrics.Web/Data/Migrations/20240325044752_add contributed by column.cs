using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ratio_Lyrics.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class addcontributedbycolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContributedBy",
                table: "SongLyrics",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContributedBy",
                table: "SongLyrics");
        }
    }
}
