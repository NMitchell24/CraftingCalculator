namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// A quantity of one <see cref="Entities.Blueprint"/> (<see cref="Child"/>) used as a component of
/// another (<see cref="ParentBlueprint"/>).
/// </summary>
public class BlueprintChild
{
    public int Id { get; set; }

    public int ParentBlueprintId { get; set; }
    public Blueprint ParentBlueprint { get; set; } = null!;

    public int ChildBlueprintId { get; set; }
    public Blueprint Child { get; set; } = null!;

    public long Quantity { get; set; }
}
