namespace CraftingCalculator.Domain.Entities;

public class Favorite : IDatasetScoped
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;

    public List<FavoriteBlueprint> FavoriteBlueprints { get; set; } = [];
}
