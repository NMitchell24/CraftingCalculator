using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class BlueprintComponentConfiguration : IEntityTypeConfiguration<BlueprintComponent>
{
    public void Configure(EntityTypeBuilder<BlueprintComponent> builder)
    {
        builder.ToTable("BlueprintComponents");
        builder.HasKey(ri => ri.Id);
        builder.HasIndex(ri => ri.BlueprintId);
        builder.HasIndex(ri => ri.ComponentId);

        builder.HasOne(ri => ri.Blueprint)
            .WithMany(r => r.Components)
            .HasForeignKey(ri => ri.BlueprintId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Component)
            .WithMany(i => i.BlueprintComponents)
            .HasForeignKey(ri => ri.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
