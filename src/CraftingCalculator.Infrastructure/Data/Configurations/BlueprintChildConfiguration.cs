using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class BlueprintChildConfiguration : IEntityTypeConfiguration<BlueprintChild>
{
    public void Configure(EntityTypeBuilder<BlueprintChild> builder)
    {
        builder.ToTable("BlueprintChildren");
        builder.HasKey(rc => rc.Id);
        builder.HasIndex(rc => rc.ParentBlueprintId);
        builder.HasIndex(rc => rc.ChildBlueprintId);

        // Two cascade paths into the same table (Blueprint) from BlueprintChild's two FKs - fine on
        // SQLite, which (unlike SQL Server) has no restriction against multiple cascade paths.
        builder.HasOne(rc => rc.ParentBlueprint)
            .WithMany(r => r.Children)
            .HasForeignKey(rc => rc.ParentBlueprintId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.Child)
            .WithMany(r => r.ParentLinks)
            .HasForeignKey(rc => rc.ChildBlueprintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
