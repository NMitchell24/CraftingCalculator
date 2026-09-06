using CraftingCalculator.Application.Common.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CraftingCalculator.Views
{
    /// <summary>
    /// Interaction logic for ConfigureRecipes.xaml
    /// </summary>
    public partial class ConfigureRecipes : UserControl
    {
        public ConfigureRecipes()
        {
            InitializeComponent();
            DataContext = new ViewModel.ConfigureRecipesViewModel(
                DialogCoordinator.Instance,
                App.Services.GetRequiredService<IIngredientService>(),
                App.Services.GetRequiredService<IRecipeFilterService>(),
                App.Services.GetRequiredService<IRecipeService>(),
                App.Services.GetRequiredService<IDatabaseAdminService>());
        }
    }
}
