using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDatasetNameIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Favorites_DatasetId",
            table: "Favorites");

        migrationBuilder.DropIndex(
            name: "IX_Components_DatasetId",
            table: "Components");

        migrationBuilder.CreateIndex(
            name: "IX_Favorites_DatasetId_Name",
            table: "Favorites",
            columns: new[] { "DatasetId", "Name" });

        migrationBuilder.CreateIndex(
            name: "IX_Components_DatasetId_Name",
            table: "Components",
            columns: new[] { "DatasetId", "Name" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Favorites_DatasetId_Name",
            table: "Favorites");

        migrationBuilder.DropIndex(
            name: "IX_Components_DatasetId_Name",
            table: "Components");

        migrationBuilder.CreateIndex(
            name: "IX_Favorites_DatasetId",
            table: "Favorites",
            column: "DatasetId");

        migrationBuilder.CreateIndex(
            name: "IX_Components_DatasetId",
            table: "Components",
            column: "DatasetId");
    }
}
