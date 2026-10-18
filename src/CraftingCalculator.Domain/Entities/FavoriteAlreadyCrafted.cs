namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// A <see cref="Entities.Blueprint"/> that a <see cref="Entities.Favorite"/>'s batch takes as already crafted rather
/// than crafting.
/// </summary>
public class FavoriteAlreadyCrafted
{
    public int Id { get; set; }

    public int FavoriteId { get; set; }
    public Favorite Favorite { get; set; } = null!;

    public int BlueprintId { get; set; }
    public Blueprint Blueprint { get; set; } = null!;
}
