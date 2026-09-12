namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// A named body of crafting data. Each dataset carries its own categories, components, blueprints and
/// favorites, which is what lets one install hold several games side by side.
/// </summary>
public class Dataset
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Category> Categories { get; set; } = [];

    public List<Component> Components { get; set; } = [];

    public List<Blueprint> Blueprints { get; set; } = [];

    public List<Favorite> Favorites { get; set; } = [];
}
