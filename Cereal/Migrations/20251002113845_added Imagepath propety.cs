using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cereal.Migrations
{
    /// <inheritdoc />
    public partial class addedImagepathpropety : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Nutritions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Nutritions");
        }
    }
}
