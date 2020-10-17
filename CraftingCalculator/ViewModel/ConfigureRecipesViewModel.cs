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

        public ConfigureRecipesViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
        }
    }
}
