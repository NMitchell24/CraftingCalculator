using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class BlueprintComponentConfiguration : IEntityTypeConfiguration<BlueprintComponent>
{
    public void Configure(EntityTypeBuilder<BlueprintComponent> builder)
    {
        builder.ToTable("BlueprintComponents");
        builder.HasKey(blueprintComponent => blueprintComponent.Id);
        builder.HasIndex(blueprintComponent => blueprintComponent.BlueprintId);
        builder.HasIndex(blueprintComponent => blueprintComponent.ComponentId);

        builder.HasOne(blueprintComponent => blueprintComponent.Blueprint)
            .WithMany(blueprint => blueprint.Components)
            .HasForeignKey(blueprintComponent => blueprintComponent.BlueprintId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(blueprintComponent => blueprintComponent.Component)
            .WithMany(component => component.BlueprintComponents)
            .HasForeignKey(blueprintComponent => blueprintComponent.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
