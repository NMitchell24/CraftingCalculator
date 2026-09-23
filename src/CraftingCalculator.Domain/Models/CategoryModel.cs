using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Simple Class to define the UI Model for Categories
/// </summary>
public class CategoryModel : IBaseDataRecord
{
    public string? Name { get; set; }
    public int Id { get; set; }
    public string? Description { get; set; }
    public DataType Type
    {
        get
        {
            return DataType.Category;
        }

        set
        {
            //Don't allow this to be changed as it should remain static
            _ = value;
        }
    }
}
