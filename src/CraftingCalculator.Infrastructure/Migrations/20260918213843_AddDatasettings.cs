using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDatasettings : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // defaultValue edited from the scaffolded false: every dataset that exists before this migration was
        // calculated with yield, so that is the setting it keeps. The model declares no store default, because
        // EF omits a bool equal to its CLR default from an INSERT and a store default of true would then
        // override a dataset saved with Use Yield off.
        migrationBuilder.AddColumn<bool>(
            name: "UseYield",
            table: "Datasets",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UseYield",
            table: "Datasets");
    }
}
