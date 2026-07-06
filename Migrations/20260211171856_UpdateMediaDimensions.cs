using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeWebSite.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaDimensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Media",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Media");
        }
    }
}
