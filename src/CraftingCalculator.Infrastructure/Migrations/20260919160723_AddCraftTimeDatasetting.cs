using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddCraftTimeDatasetting : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // defaultValue edited from the scaffolded false: every dataset that exists before this migration showed its
        // production times, so that is the setting it keeps. No store default in the model, for the reason
        // AddDatasettings gives.
        migrationBuilder.AddColumn<bool>(
            name: "UseCraftTime",
            table: "Datasets",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UseCraftTime",
            table: "Datasets");
    }
}
