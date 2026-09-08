using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddProductionTime : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "ProductionTime",
            table: "Components",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.AddColumn<long>(
            name: "ProductionTime",
            table: "Blueprints",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0L);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ProductionTime",
            table: "Components");

        migrationBuilder.DropColumn(
            name: "ProductionTime",
            table: "Blueprints");
    }
}
