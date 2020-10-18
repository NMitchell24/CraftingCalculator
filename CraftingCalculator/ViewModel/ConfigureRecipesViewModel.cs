using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.ViewModel
{
    class ConfigureRecipesViewModel : AbstractPropertyChanged
    {
        // Class level Variables used by multiple methods.
        private IDialogCoordinator dialogCoordinator;
        public List<String> DataTypeList
        {
            get
            {
                return DataType.DataTypeList;
            }
            private set { }
        }

        private String _selectedType = DataType.Ingredient;
        public String SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType == value)
                    return;

                //TODO: Else do something to reload the list of items.
            }
        }

        public ConfigureRecipesViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
        }
    }
}
