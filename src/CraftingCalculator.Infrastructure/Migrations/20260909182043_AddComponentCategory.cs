using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddComponentCategory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CategoryId",
            table: "Components",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Components_CategoryId",
            table: "Components",
            column: "CategoryId");

        migrationBuilder.AddForeignKey(
            name: "FK_Components_Categories_CategoryId",
            table: "Components",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Components_Categories_CategoryId",
            table: "Components");

        migrationBuilder.DropIndex(
            name: "IX_Components_CategoryId",
            table: "Components");

        migrationBuilder.DropColumn(
            name: "CategoryId",
            table: "Components");
    }
}
