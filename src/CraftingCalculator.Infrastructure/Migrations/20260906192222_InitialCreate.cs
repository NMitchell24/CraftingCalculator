using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Favorites",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Favorites", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Ingredients",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: false),
                Cost = table.Column<double>(type: "REAL", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Ingredients", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RecipeFilters",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RecipeFilters", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Recipes",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: false),
                Value = table.Column<double>(type: "REAL", nullable: false),
                FilterId = table.Column<int>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Recipes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Recipes_RecipeFilters_FilterId",
                    column: x => x.FilterId,
                    principalTable: "RecipeFilters",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "FavoriteRecipes",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FavoriteId = table.Column<int>(type: "INTEGER", nullable: false),
                RecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FavoriteRecipes", x => x.Id);
                table.ForeignKey(
                    name: "FK_FavoriteRecipes_Favorites_FavoriteId",
                    column: x => x.FavoriteId,
                    principalTable: "Favorites",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FavoriteRecipes_Recipes_RecipeId",
                    column: x => x.RecipeId,
                    principalTable: "Recipes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RecipeChildren",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ParentRecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                ChildRecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RecipeChildren", x => x.Id);
                table.ForeignKey(
                    name: "FK_RecipeChildren_Recipes_ChildRecipeId",
                    column: x => x.ChildRecipeId,
                    principalTable: "Recipes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RecipeChildren_Recipes_ParentRecipeId",
                    column: x => x.ParentRecipeId,
                    principalTable: "Recipes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RecipeIngredients",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                RecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                IngredientId = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                table.ForeignKey(
                    name: "FK_RecipeIngredients_Ingredients_IngredientId",
                    column: x => x.IngredientId,
                    principalTable: "Ingredients",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RecipeIngredients_Recipes_RecipeId",
                    column: x => x.RecipeId,
                    principalTable: "Recipes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_FavoriteRecipes_FavoriteId",
            table: "FavoriteRecipes",
            column: "FavoriteId");

        migrationBuilder.CreateIndex(
            name: "IX_FavoriteRecipes_RecipeId",
            table: "FavoriteRecipes",
            column: "RecipeId");

        migrationBuilder.CreateIndex(
            name: "IX_Favorites_Name",
            table: "Favorites",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Ingredients_Name",
            table: "Ingredients",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_RecipeChildren_ChildRecipeId",
            table: "RecipeChildren",
            column: "ChildRecipeId");

        migrationBuilder.CreateIndex(
            name: "IX_RecipeChildren_ParentRecipeId",
            table: "RecipeChildren",
            column: "ParentRecipeId");

        migrationBuilder.CreateIndex(
            name: "IX_RecipeFilters_Name",
            table: "RecipeFilters",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_RecipeIngredients_IngredientId",
            table: "RecipeIngredients",
            column: "IngredientId");

        migrationBuilder.CreateIndex(
            name: "IX_RecipeIngredients_RecipeId",
            table: "RecipeIngredients",
            column: "RecipeId");

        migrationBuilder.CreateIndex(
            name: "IX_Recipes_FilterId",
            table: "Recipes",
            column: "FilterId");

        migrationBuilder.CreateIndex(
            name: "IX_Recipes_Name",
            table: "Recipes",
            column: "Name");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FavoriteRecipes");

        migrationBuilder.DropTable(
            name: "RecipeChildren");

        migrationBuilder.DropTable(
            name: "RecipeIngredients");

        migrationBuilder.DropTable(
            name: "Favorites");

        migrationBuilder.DropTable(
            name: "Ingredients");

        migrationBuilder.DropTable(
            name: "Recipes");

        migrationBuilder.DropTable(
            name: "RecipeFilters");
    }
}
