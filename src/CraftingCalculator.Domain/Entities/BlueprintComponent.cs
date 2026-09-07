namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// A quantity of an <see cref="Entities.Component"/> that is a direct component of a
/// <see cref="Entities.Blueprint"/>.
/// </summary>
public class BlueprintComponent
{
    public int Id { get; set; }

    public int BlueprintId { get; set; }
    public Blueprint Blueprint { get; set; } = null!;

    public int ComponentId { get; set; }
    public Component Component { get; set; } = null!;

    public long Quantity { get; set; }
}
