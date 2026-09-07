using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class RecipeComponentConfiguration : IEntityTypeConfiguration<RecipeComponent>
{
    public void Configure(EntityTypeBuilder<RecipeComponent> builder)
    {
        builder.ToTable("RecipeComponents");
        builder.HasKey(ri => ri.Id);
        builder.HasIndex(ri => ri.RecipeId);
        builder.HasIndex(ri => ri.ComponentId);

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.Components)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Component)
            .WithMany(i => i.RecipeComponents)
            .HasForeignKey(ri => ri.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
