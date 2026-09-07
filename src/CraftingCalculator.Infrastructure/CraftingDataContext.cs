using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure;

public class CraftingDataContext(DbContextOptions<CraftingDataContext> options) : DbContext(options)
{
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeFilter> RecipeFilters => Set<RecipeFilter>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<RecipeChild> RecipeChildren => Set<RecipeChild>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<FavoriteRecipe> FavoriteRecipes => Set<FavoriteRecipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(CraftingDataContext).Assembly);
}
