namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// A quantity of one <see cref="Entities.Recipe"/> (<see cref="Child"/>) used as a component of
/// another (<see cref="ParentRecipe"/>).
/// </summary>
public class RecipeChild
{
    public int Id { get; set; }

    public int ParentRecipeId { get; set; }
    public Recipe ParentRecipe { get; set; } = null!;

    public int ChildRecipeId { get; set; }
    public Recipe Child { get; set; } = null!;

    public long Quantity { get; set; }
}
