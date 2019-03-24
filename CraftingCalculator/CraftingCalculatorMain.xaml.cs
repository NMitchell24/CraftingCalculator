using CraftingCalculator.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace CraftingCalculator
{
    /// <summary>
    /// Interaction logic for CraftingCalculatorMainWindow.xaml
    /// </summary>
    public partial class CraftingCalculatorMainWindow
    {
        public CraftingCalculatorMainWindow()
        {
            InitializeComponent();
            CraftingCalculatorMainViewModel vm = new CraftingCalculatorMainViewModel(DialogCoordinator.Instance);
            DataContext = vm;
            Closing += vm.OnWindowClosing;
        }
    }
}
