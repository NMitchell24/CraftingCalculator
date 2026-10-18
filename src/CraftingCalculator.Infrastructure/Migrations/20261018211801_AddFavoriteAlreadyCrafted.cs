using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddFavoriteAlreadyCrafted : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FavoriteAlreadyCrafted",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FavoriteId = table.Column<int>(type: "INTEGER", nullable: false),
                BlueprintId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FavoriteAlreadyCrafted", x => x.Id);
                table.ForeignKey(
                    name: "FK_FavoriteAlreadyCrafted_Blueprints_BlueprintId",
                    column: x => x.BlueprintId,
                    principalTable: "Blueprints",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FavoriteAlreadyCrafted_Favorites_FavoriteId",
                    column: x => x.FavoriteId,
                    principalTable: "Favorites",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_FavoriteAlreadyCrafted_BlueprintId",
            table: "FavoriteAlreadyCrafted",
            column: "BlueprintId");

        migrationBuilder.CreateIndex(
            name: "IX_FavoriteAlreadyCrafted_FavoriteId_BlueprintId",
            table: "FavoriteAlreadyCrafted",
            columns: new[] { "FavoriteId", "BlueprintId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FavoriteAlreadyCrafted");
    }
}
