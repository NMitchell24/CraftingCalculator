using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CraftingCalculator.Views
{
    /// <summary>
    /// Interaction logic for RecipesView.xaml
    /// </summary>
    public partial class RecipesView : UserControl
    {
        public RecipesView()
        {
            InitializeComponent();
            DataContext = new ViewModel.RecipesViewModel(DialogCoordinator.Instance);
        }
    }
}
