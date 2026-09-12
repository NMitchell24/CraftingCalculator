using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired();
        builder.HasIndex(f => f.Name);

        builder.HasIndex(f => f.DatasetId);

        //Cascade: a dataset is the container for its records, so deleting one takes its
        //contents with it.
        builder.HasOne(f => f.Dataset)
            .WithMany(dataset => dataset.Favorites)
            .HasForeignKey(f => f.DatasetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
