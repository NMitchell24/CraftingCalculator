using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A blueprint as a list shows it: its identity and category, without its components or nested blueprints.
/// Anything that needs the parts reads the <see cref="BlueprintModel"/> with the same <see cref="Id"/>.
/// </summary>
public sealed class BlueprintSummary : ICategorizedRecord
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public CategoryModel? Category { get; set; }

    public DataType Type
    {
        get => DataType.Blueprint;

        //Don't allow this to be changed as it should remain static.
        set => _ = value;
    }
}
