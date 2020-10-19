using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.ViewModel
{
    public interface IBaseDataRecord
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Tooltip { get; set; }
        DataType Type { get; set; }

        IBaseDataRecord Clone();
        IBaseDataRecord CopyForSave();

    }
}
