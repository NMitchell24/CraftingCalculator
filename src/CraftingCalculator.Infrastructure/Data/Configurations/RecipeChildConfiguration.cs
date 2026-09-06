using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class RecipeChildConfiguration : IEntityTypeConfiguration<RecipeChild>
{
    public void Configure(EntityTypeBuilder<RecipeChild> builder)
    {
        builder.ToTable("RecipeChildren");
        builder.HasKey(rc => rc.Id);
        builder.HasIndex(rc => rc.ParentRecipeId);
        builder.HasIndex(rc => rc.ChildRecipeId);

        // Two cascade paths into the same table (Recipe) from RecipeChild's two FKs - fine on
        // SQLite, which (unlike SQL Server) has no restriction against multiple cascade paths.
        builder.HasOne(rc => rc.ParentRecipe)
            .WithMany(r => r.Children)
            .HasForeignKey(rc => rc.ParentRecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.Child)
            .WithMany(r => r.ParentLinks)
            .HasForeignKey(rc => rc.ChildRecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
