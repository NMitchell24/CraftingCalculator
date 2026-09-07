using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients");
        builder.HasKey(ri => ri.Id);
        builder.HasIndex(ri => ri.RecipeId);
        builder.HasIndex(ri => ri.IngredientId);

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
