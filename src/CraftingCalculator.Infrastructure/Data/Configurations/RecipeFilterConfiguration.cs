using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class RecipeFilterConfiguration : IEntityTypeConfiguration<RecipeFilter>
{
    public void Configure(EntityTypeBuilder<RecipeFilter> builder)
    {
        builder.ToTable("RecipeFilters");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired();
        builder.Property(f => f.Description).IsRequired();
        builder.HasIndex(f => f.Name);
    }
}
