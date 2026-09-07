using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameToCraftingVocabulary : Migration
{
    // Hand-written. "dotnet ef migrations add" scaffolds this rename as 12 DropTable + 12
    // CreateTable, because the model differ matches entities by table name and so reads every
    // renamed table as one table dropped and an unrelated one added - which would delete every
    // row the user has. Rename operations carry the data across instead. The Designer file and
    // the model snapshot are still the generated ones: they describe the resulting model, which
    // is the same either way.
    //
    // Column order within a table is not preserved by the model differ's CreateTable either, but
    // RenameColumn leaves the physical order alone, so the columns keep their original positions.

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(name: "Ingredients", newName: "Components");
        migrationBuilder.RenameTable(name: "RecipeFilters", newName: "Categories");
        migrationBuilder.RenameTable(name: "Recipes", newName: "Blueprints");
        migrationBuilder.RenameTable(name: "RecipeIngredients", newName: "BlueprintComponents");
        migrationBuilder.RenameTable(name: "RecipeChildren", newName: "BlueprintChildren");
        migrationBuilder.RenameTable(name: "FavoriteRecipes", newName: "FavoriteBlueprints");

        migrationBuilder.RenameColumn(table: "Blueprints", name: "FilterId", newName: "CategoryId");
        migrationBuilder.RenameColumn(table: "BlueprintComponents", name: "RecipeId", newName: "BlueprintId");
        migrationBuilder.RenameColumn(table: "BlueprintComponents", name: "IngredientId", newName: "ComponentId");
        migrationBuilder.RenameColumn(table: "BlueprintChildren", name: "ParentRecipeId", newName: "ParentBlueprintId");
        migrationBuilder.RenameColumn(table: "BlueprintChildren", name: "ChildRecipeId", newName: "ChildBlueprintId");
        migrationBuilder.RenameColumn(table: "FavoriteBlueprints", name: "RecipeId", newName: "BlueprintId");

        migrationBuilder.RenameIndex(table: "Components", name: "IX_Ingredients_Name", newName: "IX_Components_Name");
        migrationBuilder.RenameIndex(table: "Categories", name: "IX_RecipeFilters_Name", newName: "IX_Categories_Name");
        migrationBuilder.RenameIndex(table: "Blueprints", name: "IX_Recipes_Name", newName: "IX_Blueprints_Name");
        migrationBuilder.RenameIndex(table: "Blueprints", name: "IX_Recipes_FilterId", newName: "IX_Blueprints_CategoryId");
        migrationBuilder.RenameIndex(table: "BlueprintComponents", name: "IX_RecipeIngredients_RecipeId", newName: "IX_BlueprintComponents_BlueprintId");
        migrationBuilder.RenameIndex(table: "BlueprintComponents", name: "IX_RecipeIngredients_IngredientId", newName: "IX_BlueprintComponents_ComponentId");
        migrationBuilder.RenameIndex(table: "BlueprintChildren", name: "IX_RecipeChildren_ParentRecipeId", newName: "IX_BlueprintChildren_ParentBlueprintId");
        migrationBuilder.RenameIndex(table: "BlueprintChildren", name: "IX_RecipeChildren_ChildRecipeId", newName: "IX_BlueprintChildren_ChildBlueprintId");
        migrationBuilder.RenameIndex(table: "FavoriteBlueprints", name: "IX_FavoriteRecipes_FavoriteId", newName: "IX_FavoriteBlueprints_FavoriteId");
        migrationBuilder.RenameIndex(table: "FavoriteBlueprints", name: "IX_FavoriteRecipes_RecipeId", newName: "IX_FavoriteBlueprints_BlueprintId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameIndex(table: "FavoriteBlueprints", name: "IX_FavoriteBlueprints_BlueprintId", newName: "IX_FavoriteRecipes_RecipeId");
        migrationBuilder.RenameIndex(table: "FavoriteBlueprints", name: "IX_FavoriteBlueprints_FavoriteId", newName: "IX_FavoriteRecipes_FavoriteId");
        migrationBuilder.RenameIndex(table: "BlueprintChildren", name: "IX_BlueprintChildren_ChildBlueprintId", newName: "IX_RecipeChildren_ChildRecipeId");
        migrationBuilder.RenameIndex(table: "BlueprintChildren", name: "IX_BlueprintChildren_ParentBlueprintId", newName: "IX_RecipeChildren_ParentRecipeId");
        migrationBuilder.RenameIndex(table: "BlueprintComponents", name: "IX_BlueprintComponents_ComponentId", newName: "IX_RecipeIngredients_IngredientId");
        migrationBuilder.RenameIndex(table: "BlueprintComponents", name: "IX_BlueprintComponents_BlueprintId", newName: "IX_RecipeIngredients_RecipeId");
        migrationBuilder.RenameIndex(table: "Blueprints", name: "IX_Blueprints_CategoryId", newName: "IX_Recipes_FilterId");
        migrationBuilder.RenameIndex(table: "Blueprints", name: "IX_Blueprints_Name", newName: "IX_Recipes_Name");
        migrationBuilder.RenameIndex(table: "Categories", name: "IX_Categories_Name", newName: "IX_RecipeFilters_Name");
        migrationBuilder.RenameIndex(table: "Components", name: "IX_Components_Name", newName: "IX_Ingredients_Name");

        migrationBuilder.RenameColumn(table: "FavoriteBlueprints", name: "BlueprintId", newName: "RecipeId");
        migrationBuilder.RenameColumn(table: "BlueprintChildren", name: "ChildBlueprintId", newName: "ChildRecipeId");
        migrationBuilder.RenameColumn(table: "BlueprintChildren", name: "ParentBlueprintId", newName: "ParentRecipeId");
        migrationBuilder.RenameColumn(table: "BlueprintComponents", name: "ComponentId", newName: "IngredientId");
        migrationBuilder.RenameColumn(table: "BlueprintComponents", name: "BlueprintId", newName: "RecipeId");
        migrationBuilder.RenameColumn(table: "Blueprints", name: "CategoryId", newName: "FilterId");

        migrationBuilder.RenameTable(name: "FavoriteBlueprints", newName: "FavoriteRecipes");
        migrationBuilder.RenameTable(name: "BlueprintChildren", newName: "RecipeChildren");
        migrationBuilder.RenameTable(name: "BlueprintComponents", newName: "RecipeIngredients");
        migrationBuilder.RenameTable(name: "Blueprints", newName: "Recipes");
        migrationBuilder.RenameTable(name: "Categories", newName: "RecipeFilters");
        migrationBuilder.RenameTable(name: "Components", newName: "Ingredients");
    }
}
