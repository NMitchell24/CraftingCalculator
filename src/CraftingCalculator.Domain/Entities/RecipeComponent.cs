namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// A quantity of an <see cref="Entities.Component"/> that is a direct component of a
/// <see cref="Entities.Recipe"/>.
/// </summary>
public class RecipeComponent
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int ComponentId { get; set; }
    public Component Component { get; set; } = null!;

    public long Quantity { get; set; }
}
