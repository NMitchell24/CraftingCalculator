using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddCurrencyLabelDatasetting : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // No default: null is the device's currency, which every dataset that exists before this migration showed.
        migrationBuilder.AddColumn<string>(
            name: "CurrencyLabel",
            table: "Datasets",
            type: "TEXT",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CurrencyLabel",
            table: "Datasets");
    }
}
