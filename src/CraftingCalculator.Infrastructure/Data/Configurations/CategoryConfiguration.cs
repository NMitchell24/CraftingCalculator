using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Name).IsRequired();
        builder.Property(category => category.Description).IsRequired();
        builder.HasIndex(category => category.Name);

        builder.HasIndex(category => category.DatasetId);

        //Cascade: a dataset is the container for its records, so deleting one takes its
        //contents with it.
        builder.HasOne(category => category.Dataset)
            .WithMany(dataset => dataset.Categories)
            .HasForeignKey(category => category.DatasetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
