using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class FavoriteBlueprintConfiguration : IEntityTypeConfiguration<FavoriteBlueprint>
{
    public void Configure(EntityTypeBuilder<FavoriteBlueprint> builder)
    {
        builder.ToTable("FavoriteBlueprints");
        builder.HasKey(fr => fr.Id);
        builder.HasIndex(fr => fr.FavoriteId);
        builder.HasIndex(fr => fr.BlueprintId);

        builder.HasOne(fr => fr.Favorite)
            .WithMany(f => f.FavoriteBlueprints)
            .HasForeignKey(fr => fr.FavoriteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fr => fr.Blueprint)
            .WithMany(blueprint => blueprint.FavoriteBlueprints)
            .HasForeignKey(fr => fr.BlueprintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
