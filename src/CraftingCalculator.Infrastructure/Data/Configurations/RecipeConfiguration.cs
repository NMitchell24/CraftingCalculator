using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired();
        builder.Property(r => r.Description).IsRequired();
        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.FilterId);

        builder.HasOne(r => r.Filter)
            .WithMany(f => f.Recipes)
            .HasForeignKey(r => r.FilterId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
