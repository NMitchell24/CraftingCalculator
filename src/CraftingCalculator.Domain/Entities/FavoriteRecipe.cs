namespace CraftingCalculator.Domain.Entities;

/// <summary>A saved quantity of a <see cref="Entities.Recipe"/> within a <see cref="Entities.Favorite"/>.</summary>
public class FavoriteRecipe
{
    public int Id { get; set; }

    public int FavoriteId { get; set; }
    public Favorite Favorite { get; set; } = null!;

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public long Quantity { get; set; }
}
