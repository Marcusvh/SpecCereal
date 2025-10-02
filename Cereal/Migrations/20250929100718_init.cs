using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cereal.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Nutritions",
                columns: table => new
                {
                    Cereal_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mfr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Protein = table.Column<int>(type: "int", nullable: false),
                    Fat = table.Column<int>(type: "int", nullable: false),
                    Sodium = table.Column<int>(type: "int", nullable: false),
                    Fiber = table.Column<float>(type: "float", nullable: false),
                    Carbo = table.Column<float>(type: "float", nullable: false),
                    Sugars = table.Column<int>(type: "int", nullable: false),
                    Potass = table.Column<int>(type: "int", nullable: false),
                    Vitamins = table.Column<int>(type: "int", nullable: false),
                    Shelf = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<float>(type: "float", nullable: false),
                    Cups = table.Column<float>(type: "float", nullable: false),
                    Rating = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutritions", x => x.Cereal_Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nutritions");
        }
    }
}
