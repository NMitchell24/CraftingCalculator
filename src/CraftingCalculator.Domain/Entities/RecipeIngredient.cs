namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// A quantity of an <see cref="Entities.Ingredient"/> that is a direct component of a
/// <see cref="Entities.Recipe"/>.
/// </summary>
public class RecipeIngredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public long Quantity { get; set; }
}
