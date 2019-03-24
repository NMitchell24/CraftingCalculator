using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes;
using CraftingCalculator.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace CraftingCalculator.ViewModel
{
    public class RecipesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Recipe> RecipesList { get; set; }
        public List<RecipeFilter> RecipeFilters { get; set; }

        private RecipeMap _recipeMap = new RecipeMap();
        private IngredientMap _ingredientMap = new IngredientMap();

        public CommandRunner AddRecipeCommand { get; set; }
        public CommandRunner RemoveRecipeCommand { get; set; }
        public CommandRunner RecalculateTotalsCommand { get; set; }
        public CommandRunner ClearQuantitiesCommand { get; set; }
        public CommandRunner CopyIngredientsCommand { get; set; }

        public ObservableCollection<RecipeQuantity> RecipeQuantities { get; set; }
        public ObservableCollection<IngredientQuantity> TotalIngredients { get; set; }
        public ObservableCollection<RecipeTree> RecipeTotals { get; set; }

        private bool CanAddRecipes()
        {
            return RecipesList.Where(x => x.IsSelected).Count() > 0;
        }

        private void AddRecipes()
        {
            foreach(Recipe recipe in RecipesList.Where(x => x.IsSelected))
            {
                // Add the recipe or increase the quantity.
                _recipeMap.Add(recipe, 1);
                 
                // need to deselect recipe so that the ListBox does not leave them selected when they are out of view.
                recipe.IsSelected = false;
            }

            RecipeQuantities = new ObservableCollection<RecipeQuantity>(_recipeMap.RecipeList);

            CalculateTotalIngredients();

            RaisePropertyChanged(nameof(RecipeQuantities));
            RaisePropertyChanged(nameof(SelectedRecipe));
            RaisePropertyChanged(nameof(RecipesList));
            RemoveRecipeCommand.RaiseCanExecuteChanged();
            ClearQuantitiesCommand.RaiseCanExecuteChanged();
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

            CalculateTotalIngredients();

            RaisePropertyChanged(nameof(RecipeQuantities));
            RaisePropertyChanged(nameof(SelectedRecipe));
            RemoveRecipeCommand.RaiseCanExecuteChanged();
            ClearQuantitiesCommand.RaiseCanExecuteChanged();
        }

        private void CalculateTotalIngredients()
        {
            _ingredientMap.Reset();
            foreach(RecipeQuantity q in RecipeQuantities)
            {
                foreach(IngredientQuantity i in q.Ingredients.IngredientList)
                {
                    _ingredientMap.Add(i.Ingredient, (i.Quantity * q.Quantity));
                }
            }
            TotalIngredients = new ObservableCollection<IngredientQuantity>(_ingredientMap.IngredientList);
            RaisePropertyChanged(nameof(TotalIngredients));
            CopyIngredientsCommand.RaiseCanExecuteChanged();

            // rebuild recipe tree
            CalculateRecipeTotals();
        }

        private void CalculateRecipeTotals()
        {
            RecipeTotals.Clear();
            foreach(RecipeQuantity q in RecipeQuantities)
            {
                RecipeTotals.Add(q.Recipe.GetRecipeNodes(q.Quantity));
            }
            foreach(RecipeTree t in RecipeTotals)
            {
                t.SetParent();
            }
            RaisePropertyChanged(nameof(RecipeTotals));
        }

        private bool CanClearRecipes()
        {
            return RecipeQuantities != null && RecipeQuantities.Count() > 0;
        }

        private void ClearRecipes()
        {
            _recipeMap.Reset();
            RecipeQuantities.Clear();

            CalculateTotalIngredients();
            RaisePropertyChanged(nameof(RecipeQuantities));
            RaisePropertyChanged(nameof(SelectedRecipe));
            RemoveRecipeCommand.RaiseCanExecuteChanged();
            ClearQuantitiesCommand.RaiseCanExecuteChanged();
        }

        private bool CanCopyIngredients()
        {
            return TotalIngredients != null && TotalIngredients.Count() > 0;
        }

        private void CopyIngredientsToClipboard()
        {
            StringBuilder sb = new StringBuilder();

            // TotalIngredients.ToList().ForEach(i => sb.AppendLine(i.DisplayName));
            sb.Append(string.Join("\n", TotalIngredients.Select(i => i.DisplayName)));

            Clipboard.SetText(sb.ToString());
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
                AddRecipeCommand.RaiseCanExecuteChanged();
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
                RemoveRecipeCommand.RaiseCanExecuteChanged();
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
            RecipeTotals = new ObservableCollection<RecipeTree>();
            AddRecipeCommand = new CommandRunner(AddRecipes, CanAddRecipes);
            RemoveRecipeCommand = new CommandRunner(RemoveRecipes, CanRemoveRecipes);
            RecalculateTotalsCommand = new CommandRunner(CalculateTotalIngredients);
            ClearQuantitiesCommand = new CommandRunner(ClearRecipes, CanClearRecipes);
            CopyIngredientsCommand = new CommandRunner(CopyIngredientsToClipboard, CanCopyIngredients);
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
