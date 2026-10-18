using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddSharedSurplusDatasetting : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // defaultValue edited from the scaffolded false: shared surplus is on for a new dataset, and every dataset that
        // exists before this migration gets it too. No store default in the model, for the reason AddDatasettings gives.
        migrationBuilder.AddColumn<bool>(
            name: "UseSharedSurplus",
            table: "Datasets",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UseSharedSurplus",
            table: "Datasets");
    }
}
