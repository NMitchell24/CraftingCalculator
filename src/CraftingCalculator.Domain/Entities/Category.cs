namespace CraftingCalculator.Domain.Entities;

public class Category : IDatasetScoped
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;

    public List<Blueprint> Blueprints { get; set; } = [];

    public List<Component> Components { get; set; } = [];
}
