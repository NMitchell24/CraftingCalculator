using AwesomeAssertions;
using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// One test per row of the plan's delete-behavior table: each cascade/SetNull configuration replaces
/// a hand-written cleanup loop that used to walk LiteDB collections, so these are the direct
/// regression net for that removed code.
/// </summary>
[TestFixture]
public class DeleteBehaviorTests
{
    private SqliteTestFixture _fixture = null!;

    [SetUp]
    public void SetUp() => _fixture = new SqliteTestFixture();

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task DeletingRecipe_CascadesItsOwnIngredients()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Recipe recipe = new() { Name = "Plank" };
        recipe.Ingredients.Add(new RecipeIngredient { Ingredient = new Ingredient { Name = "Wood" }, Quantity = 2 });
        seed.Recipes.Add(recipe);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Recipes.Where(r => r.Id == recipe.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.RecipeIngredients.CountAsync(ri => ri.RecipeId == recipe.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingIngredient_CascadesRecipeIngredientsReferencingIt()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Ingredient ingredient = new() { Name = "Wood" };
        Recipe recipe = new() { Name = "Plank" };
        recipe.Ingredients.Add(new RecipeIngredient { Ingredient = ingredient, Quantity = 2 });
        seed.Recipes.Add(recipe);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Ingredients.Where(i => i.Id == ingredient.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.RecipeIngredients.CountAsync(ri => ri.IngredientId == ingredient.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingParentRecipe_CascadesItsChildLinks()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Recipe child = new() { Name = "Plank" };
        Recipe parent = new() { Name = "Table" };
        parent.Children.Add(new RecipeChild { Child = child, Quantity = 4 });
        seed.Recipes.AddRange(parent, child);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Recipes.Where(r => r.Id == parent.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.RecipeChildren.CountAsync(rc => rc.ParentRecipeId == parent.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingChildRecipe_CascadesLinksFromRecipesThatUseItAsAComponent()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Recipe child = new() { Name = "Plank" };
        Recipe parent = new() { Name = "Table" };
        parent.Children.Add(new RecipeChild { Child = child, Quantity = 4 });
        seed.Recipes.AddRange(parent, child);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Recipes.Where(r => r.Id == child.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.RecipeChildren.CountAsync(rc => rc.ChildRecipeId == child.Id)).Should().Be(0);
        // The parent recipe itself must survive - only the link to its now-deleted component goes.
        (await verify.Recipes.AnyAsync(r => r.Id == parent.Id)).Should().BeTrue();
    }

    [Test]
    public async Task DeletingFilter_SetsRecipesFilterIdToNullRatherThanDeletingThem()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        RecipeFilter filter = new() { Name = "Building" };
        Recipe recipe = new() { Name = "Table", Filter = filter };
        seed.Recipes.Add(recipe);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.RecipeFilters.Where(f => f.Id == filter.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        Recipe? reloaded = await verify.Recipes.FirstOrDefaultAsync(r => r.Id == recipe.Id);
        reloaded.Should().NotBeNull();
        reloaded!.FilterId.Should().BeNull();
    }

    [Test]
    public async Task DeletingFavorite_CascadesItsSavedRecipeQuantities()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Recipe recipe = new() { Name = "Table" };
        Favorite favorite = new() { Name = "Weekly Batch" };
        favorite.FavoriteRecipes.Add(new FavoriteRecipe { Recipe = recipe, Quantity = 3 });
        seed.Favorites.Add(favorite);
        seed.Recipes.Add(recipe);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Favorites.Where(f => f.Id == favorite.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.FavoriteRecipes.CountAsync(fr => fr.FavoriteId == favorite.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingRecipe_CascadesFavoriteEntriesReferencingIt()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Recipe recipe = new() { Name = "Table" };
        Favorite favorite = new() { Name = "Weekly Batch" };
        favorite.FavoriteRecipes.Add(new FavoriteRecipe { Recipe = recipe, Quantity = 3 });
        seed.Favorites.Add(favorite);
        seed.Recipes.Add(recipe);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Recipes.Where(r => r.Id == recipe.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.FavoriteRecipes.CountAsync(fr => fr.RecipeId == recipe.Id)).Should().Be(0);
        // The favorite itself survives - only its entry for the deleted recipe goes.
        (await verify.Favorites.AnyAsync(f => f.Id == favorite.Id)).Should().BeTrue();
    }
}
