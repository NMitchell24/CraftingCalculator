using CraftingCalculator.Domain.Enums;
using System;

namespace CraftingCalculator.Domain.Models;

public interface IBaseQuantityRecord
{
    string Name { get; set; }
    long Quantity { get; set; }
    string Description { get; set; }
    String Tooltip { get; set; }
    DataType Type { get; set; }
}
