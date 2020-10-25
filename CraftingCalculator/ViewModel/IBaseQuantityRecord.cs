using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.ViewModel
{
    public interface IBaseQuantityRecord
    {
        string Name { get; set; }
        long Quantity { get; set; }
        string Description { get; set; }
        String Tooltip { get; set; }
        DataType Type { get; set; }
    }
}
