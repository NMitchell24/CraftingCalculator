using CraftingCalculator.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.Utilities
{
    public static class DataTypeUtil
    {
        public static IEnumerable<DataType> GetDataTypeList()
        {
            {
                return Enum.GetValues(typeof(DataType)).Cast<DataType>();
            }
        }
    }
}
