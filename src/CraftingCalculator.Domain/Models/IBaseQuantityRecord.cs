using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

public interface IBaseQuantityRecord
{
    string Name { get; set; }
    long Quantity { get; set; }
    string Description { get; set; }
    DataType Type { get; set; }

    /// <summary>The component or blueprint this is a quantity of.</summary>
    IBaseDataRecord Record { get; }
}
