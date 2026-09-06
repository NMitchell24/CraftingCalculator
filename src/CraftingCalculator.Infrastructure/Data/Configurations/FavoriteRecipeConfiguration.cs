using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class FavoriteRecipeConfiguration : IEntityTypeConfiguration<FavoriteRecipe>
{
    public void Configure(EntityTypeBuilder<FavoriteRecipe> builder)
    {
        builder.ToTable("FavoriteRecipes");
        builder.HasKey(fr => fr.Id);
        builder.HasIndex(fr => fr.FavoriteId);
        builder.HasIndex(fr => fr.RecipeId);

        builder.HasOne(fr => fr.Favorite)
            .WithMany(f => f.FavoriteRecipes)
            .HasForeignKey(fr => fr.FavoriteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fr => fr.Recipe)
            .WithMany(r => r.FavoriteRecipes)
            .HasForeignKey(fr => fr.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
