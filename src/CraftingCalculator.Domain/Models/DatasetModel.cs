namespace CraftingCalculator.Domain.Models;

/// <summary>
/// The UI model for a dataset. Deliberately not an <see cref="IBaseDataRecord"/>: that interface is the
/// contract for the three record types the Dataset screen edits, and a dataset is the container those
/// records live in rather than one of them.
/// </summary>
public class DatasetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
