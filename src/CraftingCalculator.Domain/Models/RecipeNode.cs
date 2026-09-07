namespace CraftingCalculator.Domain.Models;

/// <summary>
/// One node in a recipe's component breakdown tree: either the recipe itself, one of its nested
/// child recipes, or a leaf ingredient. <see cref="Id"/> matches the source recipe/ingredient's
/// <c>Name</c>, not its numeric id - same-named recipes collide, matching the WPF app's tree.
/// </summary>
public sealed record RecipeNode(string Name, string? Id, string Tooltip, bool IsIngredient, IReadOnlyList<RecipeNode> Children);
