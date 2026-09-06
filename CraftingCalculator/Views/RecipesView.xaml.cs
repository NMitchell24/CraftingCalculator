using System.Windows.Controls;
using CraftingCalculator.Application.Common.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.DependencyInjection;

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
            DataContext = new ViewModel.RecipesViewModel(
                DialogCoordinator.Instance,
                App.Services.GetRequiredService<IRecipeFilterService>(),
                App.Services.GetRequiredService<IRecipeService>(),
                App.Services.GetRequiredService<IFavoriteService>());
        }
    }
}
