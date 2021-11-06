using CraftingCalculator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
