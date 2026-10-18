using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class FavoriteAlreadyCraftedConfiguration : IEntityTypeConfiguration<FavoriteAlreadyCrafted>
{
    public void Configure(EntityTypeBuilder<FavoriteAlreadyCrafted> builder)
    {
        builder.ToTable("FavoriteAlreadyCrafted");
        builder.HasKey(link => link.Id);
        builder.HasIndex(link => new { link.FavoriteId, link.BlueprintId }).IsUnique();
        builder.HasIndex(link => link.BlueprintId);

        builder.HasOne(link => link.Favorite)
            .WithMany(favorite => favorite.AlreadyCrafted)
            .HasForeignKey(link => link.FavoriteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(link => link.Blueprint)
            .WithMany(blueprint => blueprint.FavoriteAlreadyCrafted)
            .HasForeignKey(link => link.BlueprintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
