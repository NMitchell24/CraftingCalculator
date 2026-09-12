namespace CraftingCalculator.Domain.Entities;

/// <summary>
/// An entity that belongs to exactly one <see cref="Dataset"/>. Implementing this is what puts a new
/// entity under the dataset query filter and the insert-time stamping.
/// </summary>
public interface IDatasetScoped
{
    int DatasetId { get; set; }
}
