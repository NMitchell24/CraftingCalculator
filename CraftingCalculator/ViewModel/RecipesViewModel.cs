using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CraftingCalculator.Utilities;
using CraftingCalculator.Model.Recipes;
using System.Linq;

namespace CraftingCalculator.ViewModel
{
    public class RecipesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Recipe> RecipesList { get; set; }
        public List<RecipeFilter> RecipeFilters { get; set; }
        private RecipeMap _recipeMap = new RecipeMap();
        public CommandRunner AddRecipeCommand { get; set; }
        public CommandRunner RemoveRecipeCommand { get; set; }

        public ObservableCollection<RecipeQuantity> RecipeQuantities { get; set; }

        private bool CanAddRecipes()
        {
            return RecipesList.Where(x => x.IsSelected).Count() > 0;
        }

        private void AddRecipes()
        {
            foreach(Recipe recipe in RecipesList.Where(x => x.IsSelected))
            {
                // Add or adjust recipe quantity.
                _recipeMap.Add(recipe, 1);
            }

            RecipeQuantities = new ObservableCollection<RecipeQuantity>(_recipeMap.RecipeList);
            RaisePropertyChanged(nameof(RecipeQuantities));
            RemoveRecipeCommand.RaiseCanExecuteChanged();
        }

        private bool CanRemoveRecipes()
        {
            return RecipeQuantities != null && RecipeQuantities.Where(x => x.IsSelected).Count() > 0;
        }

        private void RemoveRecipes()
        {
            foreach(RecipeQuantity recipe in RecipeQuantities.Where(x => x.IsSelected))
            {
                _recipeMap.RemoveAll(recipe.Recipe);
            }
            RecipeQuantities = new ObservableCollection<RecipeQuantity>(_recipeMap.RecipeList);
            RaisePropertyChanged(nameof(RecipeQuantities));
            RemoveRecipeCommand.RaiseCanExecuteChanged();
        }

        private RecipeFilter _selectedFilter;
        public RecipeFilter SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter == value)
                    return;

                _selectedFilter = value;

                ReloadRecipesForFilter(_selectedFilter);
            }
        }

        private Recipe _selectedRecipe;
        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                var selectedItems = RecipesList.Where(x => x.IsSelected).Count();
                RaisePropertyChanged(nameof(SelectedRecipe));
                RaisePropertyChanged(nameof(RecipesList));
            }
        }

        private RecipeQuantity _selectedRecipeQuantity;
        public RecipeQuantity SelectedRecipeQuantity
        {
            get => _selectedRecipeQuantity;
            set
            {
                var selectedItems = RecipeQuantities.Where(x => x.IsSelected).Count();
                RaisePropertyChanged(nameof(SelectedRecipeQuantity));
                RaisePropertyChanged(nameof(RecipeQuantities));
            }
        }
        
        // Have to bind to selected index so that we can disable the Add button when 
        // User Deselects all recipes
        private int _selectedRecipeIndex;
        public int SelectedRecipeIndex
        {
            get => _selectedRecipeIndex;
            set
            {
                _selectedRecipeIndex = value;
                AddRecipeCommand.RaiseCanExecuteChanged();
            }
        }

        private int _selectedRecipeQuantityIndex;
        public int SelectedRecipeQuantityIndex
        {
            get => _selectedRecipeQuantityIndex;
            set
            {
                _selectedRecipeQuantityIndex = value;
                RemoveRecipeCommand.RaiseCanExecuteChanged();
            }
        }

        public RecipesViewModel()
        {
            RecipeFilters = RecipeUtil.GetRecipeFilters();
            SelectedFilter = RecipeFilters[0];
            RecipesList = new ObservableCollection<Recipe>(RecipeUtil.GetRecipesByFilter(SelectedFilter));
            AddRecipeCommand = new CommandRunner(AddRecipes, CanAddRecipes);
            RemoveRecipeCommand = new CommandRunner(RemoveRecipes, CanRemoveRecipes);
        }

        public void ReloadRecipesForFilter(RecipeFilter filter)
        {
            RecipesList = new ObservableCollection<Recipe>(RecipeUtil.GetRecipesByFilter(filter));
            RaisePropertyChanged(nameof(RecipesList));
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
