using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddEconomyDatasettings : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // defaultValue edited from the scaffolded false: every dataset that exists before this migration was
        // calculated with costs and values, so that is the setting it keeps. No store default in the model, for the
        // reason AddDatasettings gives.
        migrationBuilder.AddColumn<bool>(
            name: "UseCosts",
            table: "Datasets",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<bool>(
            name: "UseValues",
            table: "Datasets",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UseCosts",
            table: "Datasets");

        migrationBuilder.DropColumn(
            name: "UseValues",
            table: "Datasets");
    }
}
