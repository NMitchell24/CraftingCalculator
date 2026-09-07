using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure;

public class CraftingDataContext(DbContextOptions<CraftingDataContext> options) : DbContext(options)
{
    public DbSet<Component> Components => Set<Component>();
    public DbSet<Category> Categorys => Set<Category>();
    public DbSet<Blueprint> Blueprints => Set<Blueprint>();
    public DbSet<BlueprintComponent> BlueprintComponents => Set<BlueprintComponent>();
    public DbSet<BlueprintChild> BlueprintChildren => Set<BlueprintChild>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<FavoriteBlueprint> FavoriteBlueprints => Set<FavoriteBlueprint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(CraftingDataContext).Assembly);
}
