using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

public interface IBaseQuantityRecord
{
    string Name { get; set; }
    long Quantity { get; set; }
    string Description { get; set; }
    DataType Type { get; set; }
}
