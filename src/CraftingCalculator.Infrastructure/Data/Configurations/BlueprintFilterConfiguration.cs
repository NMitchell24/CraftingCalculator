using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class BlueprintFilterConfiguration : IEntityTypeConfiguration<BlueprintFilter>
{
    public void Configure(EntityTypeBuilder<BlueprintFilter> builder)
    {
        builder.ToTable("BlueprintFilters");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired();
        builder.Property(f => f.Description).IsRequired();
        builder.HasIndex(f => f.Name);
    }
}
