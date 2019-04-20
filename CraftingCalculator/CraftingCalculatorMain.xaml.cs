using CraftingCalculator.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using CraftingCalculator.Service;

namespace CraftingCalculator
{
    /// <summary>
    /// Interaction logic for CraftingCalculatorMainWindow.xaml
    /// </summary>
    public partial class CraftingCalculatorMainWindow
    {
        public CraftingCalculatorMainWindow()
        {
            //Must ensure the DB exists before starting the application.
            DatabaseCreationService.CreateDatabase();
            InitializeComponent();
            CraftingCalculatorMainViewModel vm = new CraftingCalculatorMainViewModel(DialogCoordinator.Instance);
            DataContext = vm;
            Closing += vm.OnWindowClosing;
        }
    }
}
