using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DropNameOnlyIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Favorites_Name",
            table: "Favorites");

        migrationBuilder.DropIndex(
            name: "IX_Components_Name",
            table: "Components");

        migrationBuilder.DropIndex(
            name: "IX_Categories_Name",
            table: "Categories");

        migrationBuilder.DropIndex(
            name: "IX_Blueprints_Name",
            table: "Blueprints");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Favorites_Name",
            table: "Favorites",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Components_Name",
            table: "Components",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Categories_Name",
            table: "Categories",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Blueprints_Name",
            table: "Blueprints",
            column: "Name");
    }
}
