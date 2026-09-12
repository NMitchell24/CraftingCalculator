using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CraftingCalculator.Infrastructure;

public class CraftingDataContext(DbContextOptions<CraftingDataContext> options) : DbContext(options)
{
    public DbSet<Dataset> Datasets => Set<Dataset>();
    public DbSet<Component> Components => Set<Component>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Blueprint> Blueprints => Set<Blueprint>();
    public DbSet<BlueprintComponent> BlueprintComponents => Set<BlueprintComponent>();
    public DbSet<BlueprintChild> BlueprintChildren => Set<BlueprintChild>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<FavoriteBlueprint> FavoriteBlueprints => Set<FavoriteBlueprint>();

    /// <summary>
    /// The dataset every query on this context is scoped to, and the one new records are filed under.
    /// Set by <see cref="DatasetScopedContextFactory"/>, which is how every DAO but
    /// <see cref="DAO.Impl.DatasetDAO"/> obtains a context.
    /// </summary>
    public int DatasetId { get; set; }

    public override int SaveChanges()
    {
        StampDataset();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampDataset();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CraftingDataContext).Assembly);

        // Declared here rather than in the IEntityTypeConfiguration classes because the filter has to
        // close over this context instance's DatasetId, which Configure() has no access to. EF reads
        // the property as a query parameter, so the compiled model stays shared across contexts and a
        // pooled context picks up whatever DatasetId it was handed.
        modelBuilder.Entity<Category>().HasQueryFilter(category => category.DatasetId == DatasetId);
        modelBuilder.Entity<Component>().HasQueryFilter(component => component.DatasetId == DatasetId);
        modelBuilder.Entity<Blueprint>().HasQueryFilter(blueprint => blueprint.DatasetId == DatasetId);
        modelBuilder.Entity<Favorite>().HasQueryFilter(favorite => favorite.DatasetId == DatasetId);

        // The link tables reach the dataset through their parent rather than carrying a column of their
        // own, which would be a second copy of the same fact and could drift from it. Filtering them is
        // what scopes the direct BlueprintComponents/BlueprintChildren reads in BlueprintDAO's
        // LoadGraphAsync, and it also satisfies EF's requirement that a required dependent of a
        // filtered principal be filtered too.
        modelBuilder.Entity<BlueprintComponent>().HasQueryFilter(link => link.Blueprint.DatasetId == DatasetId);
        modelBuilder.Entity<BlueprintChild>().HasQueryFilter(link => link.ParentBlueprint.DatasetId == DatasetId);
        modelBuilder.Entity<FavoriteBlueprint>().HasQueryFilter(link => link.Favorite.DatasetId == DatasetId);
    }

    /// <summary>Files new records under this context's dataset, so no DAO has to set it.</summary>
    private void StampDataset()
    {
        foreach (EntityEntry<IDatasetScoped> entry in ChangeTracker.Entries<IDatasetScoped>())
        {
            // An entity that already names a dataset is left alone: the only writer of a non-zero value
            // is a caller filing a record into a dataset other than the current one, which is what an
            // import will do.
            if (entry.State == EntityState.Added && entry.Entity.DatasetId == 0)
            {
                entry.Entity.DatasetId = DatasetId;
            }
        }
    }
}
