namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A record the user can file under a <see cref="CategoryModel"/>. Implemented by
/// <see cref="BlueprintModel"/> and <see cref="ComponentModel"/>. A category is not itself filed
/// under a category, so <see cref="CategoryModel"/> does not implement this.
/// </summary>
public interface ICategorizedRecord : IBaseDataRecord
{
    /// <summary>The category this record is filed under, or null when it has none.</summary>
    CategoryModel? Category { get; set; }
}
