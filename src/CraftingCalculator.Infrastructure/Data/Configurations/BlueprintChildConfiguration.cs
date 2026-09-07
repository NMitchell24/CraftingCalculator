using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class BlueprintChildConfiguration : IEntityTypeConfiguration<BlueprintChild>
{
    public void Configure(EntityTypeBuilder<BlueprintChild> builder)
    {
        builder.ToTable("BlueprintChildren");
        builder.HasKey(blueprintChild => blueprintChild.Id);
        builder.HasIndex(blueprintChild => blueprintChild.ParentBlueprintId);
        builder.HasIndex(blueprintChild => blueprintChild.ChildBlueprintId);

        // Two cascade paths into the same table (Blueprint) from BlueprintChild's two FKs - fine on
        // SQLite, which (unlike SQL Server) has no restriction against multiple cascade paths.
        builder.HasOne(blueprintChild => blueprintChild.ParentBlueprint)
            .WithMany(blueprint => blueprint.Children)
            .HasForeignKey(blueprintChild => blueprintChild.ParentBlueprintId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(blueprintChild => blueprintChild.Child)
            .WithMany(blueprint => blueprint.ParentLinks)
            .HasForeignKey(blueprintChild => blueprintChild.ChildBlueprintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
