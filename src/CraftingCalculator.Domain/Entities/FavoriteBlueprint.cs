namespace CraftingCalculator.Domain.Entities;

/// <summary>A saved quantity of a <see cref="Entities.Blueprint"/> within a <see cref="Entities.Favorite"/>.</summary>
public class FavoriteBlueprint
{
    public int Id { get; set; }

    public int FavoriteId { get; set; }
    public Favorite Favorite { get; set; } = null!;

    public int BlueprintId { get; set; }
    public Blueprint Blueprint { get; set; } = null!;

    public long Quantity { get; set; }
}
