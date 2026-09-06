using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Application.Common.Utils;

public static class DataTypeUtil
{
    public static IEnumerable<DataType> GetDataTypeList()
    {
        return Enum.GetValues(typeof(DataType)).Cast<DataType>();
    }
}
