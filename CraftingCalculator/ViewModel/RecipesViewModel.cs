using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes;
using CraftingCalculator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace CraftingCalculator.ViewModel
{
    public class RecipesViewModel : INotifyPropertyChanged
    {
        private IDialogCoordinator dialogCoordinator;
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
        public CommandRunner SaveRecipeFavoritesCommand { get; set; }

        public ObservableCollection<RecipeQuantity> RecipeQuantities { get; set; }
        public ObservableCollection<IngredientQuantity> TotalIngredients { get; set; }
        public ObservableCollection<RecipeTree> RecipeTotals { get; set; }
        public ObservableCollection<RecipeFavorite> RecipeFavorites { get; set; }

        private bool CanAddRecipes()
        {
            return RecipesList.Where(x => x.IsSelected).Count() > 0;
        }

        private void AddRecipes(object obj)
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
            SaveRecipeFavoritesCommand.RaiseCanExecuteChanged();
            SelectedFav = null;
            RaisePropertyChanged(nameof(RecipeFavorites));
        }

        private bool CanRemoveRecipes()
        {
            return RecipeQuantities != null && RecipeQuantities.Where(x => x.IsSelected).Count() > 0;
        }

        private void RemoveRecipes(object obj)
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
            SaveRecipeFavoritesCommand.RaiseCanExecuteChanged();
            SelectedFav = null;
            RaisePropertyChanged(nameof(RecipeFavorites));
        }

        private void CalculateTotalIngredients(object obj = null)
        {
            _ingredientMap.Reset();
            foreach(RecipeQuantity q in RecipeQuantities)
            {
                foreach(IngredientQuantity i in q.Ingredients.IngredientList)
                {
                    _ingredientMap.Add(i.Name, i.Description, (i.Quantity * q.Quantity));
                }
            }
            TotalIngredients = new ObservableCollection<IngredientQuantity>(_ingredientMap.IngredientList.OrderBy(x => x.Name));
            RaisePropertyChanged(nameof(TotalIngredients));
            CopyIngredientsCommand.RaiseCanExecuteChanged();

            // rebuild recipe tree
            CalculateRecipeTotals();
        }

        private void CalculateRecipeTotals()
        {
            List<RecipeTree> temp = new List<RecipeTree>();
            foreach(RecipeQuantity q in RecipeQuantities)
            {
                RecipeTree tree = q.Recipe.GetRecipeNodes(q.Quantity);
                RecipeTree oldTree = RecipeTotals.Where(x => x.Id == tree.Id).FirstOrDefault();
                if(oldTree != null)
                {
                    tree.SetExpandedNodes(oldTree);
                }
                temp.Add(tree);
            }
            foreach(RecipeTree t in temp)
            {
                t.SetParent();
            }

            RecipeTotals = new ObservableCollection<RecipeTree>(temp);
            RaisePropertyChanged(nameof(RecipeTotals));
        }

        private bool IsRecipesExist()
        {
            return RecipeQuantities != null && RecipeQuantities.Count() > 0;
        }

        private bool CanClearRecipes()
        {
            return IsRecipesExist();
        }

        private void ClearRecipes(object obj)
        {
            _recipeMap.Reset();
            RecipeQuantities.Clear();

            CalculateTotalIngredients();
            RaisePropertyChanged(nameof(RecipeQuantities));
            RaisePropertyChanged(nameof(SelectedRecipe));
            RemoveRecipeCommand.RaiseCanExecuteChanged();
            ClearQuantitiesCommand.RaiseCanExecuteChanged();
            SaveRecipeFavoritesCommand.RaiseCanExecuteChanged();
            SelectedFav = null;
            RaisePropertyChanged(nameof(RecipeFavorites));
        }

        private RecipeFavorite _selectedFav;
        public RecipeFavorite SelectedFav
        {
            get => _selectedFav;
            set
            {
                if (_selectedFav == value)
                    return;
                _selectedFav = value;

                if (value != null)
                {
                    _recipeMap.Reset();
                    List<RecipeQuantity> quans = RecipeUtil.GetRecipeQuantitiesForFavorite(_selectedFav);
                    foreach (RecipeQuantity q in quans)
                    {
                        _recipeMap.Add(q.Recipe, q.Quantity);
                    }

                    RecipeQuantities = new ObservableCollection<RecipeQuantity>(_recipeMap.RecipeList);

                    CalculateTotalIngredients();
                    RaisePropertyChanged(nameof(RecipeQuantities));
                    RaisePropertyChanged(nameof(SelectedRecipe));
                    RaisePropertyChanged(nameof(RecipesList));
                    RemoveRecipeCommand.RaiseCanExecuteChanged();
                    ClearQuantitiesCommand.RaiseCanExecuteChanged();
                    SaveRecipeFavoritesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanSaveRecipes()
        {
            return IsRecipesExist();
        }

        private async void SaveRecipes(object obj)
        {
            var name = await dialogCoordinator
                .ShowInputAsync(this,"Save Recipe Favorites", "Enter a Name for this Favorite:");

            bool doSave = true;
            if (RecipeUtil.DoesFavoriteExist(name))
            {
                var settings = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" };
                var yesNo = await dialogCoordinator.ShowMessageAsync(this, "Confirm",
                "Recipe Favorite already exists, do you want to overwrite?",
                MessageDialogStyle.AffirmativeAndNegative,
                settings);

                doSave = yesNo == MessageDialogResult.Affirmative;
            }

            if (doSave)
            {
                RecipeFavorite fav = new RecipeFavorite()
                {
                    Name = name
                };
                RecipeUtil.SaveRecipeFavorite(fav, RecipeQuantities.ToList());
            }

            RecipeFavorites = new ObservableCollection<RecipeFavorite>(RecipeUtil.GetAllRecipeFavorites());
            RaisePropertyChanged(nameof(RecipeFavorites));
        }

        private bool CanCopyIngredients()
        {
            return TotalIngredients != null && TotalIngredients.Count() > 0;
        }

        private void CopyIngredientsToClipboard(object obj)
        {
            StringBuilder sb = new StringBuilder();

            var displayNames = TotalIngredients.Select(i => i.DisplayName);
            sb.Append(string.Join(Environment.NewLine, displayNames));

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

        public RecipesViewModel(IDialogCoordinator instance)
        {
            RecipeFilters = RecipeUtil.GetRecipeFilters();
            SelectedFilter = RecipeFilters[0];
            RecipesList = new ObservableCollection<Recipe>(RecipeUtil.GetRecipesByFilter(SelectedFilter));
            RecipeFavorites = new ObservableCollection<RecipeFavorite>(RecipeUtil.GetAllRecipeFavorites());
            RecipeTotals = new ObservableCollection<RecipeTree>();
            AddRecipeCommand = new CommandRunner(AddRecipes, CanAddRecipes);
            RemoveRecipeCommand = new CommandRunner(RemoveRecipes, CanRemoveRecipes);
            RecalculateTotalsCommand = new CommandRunner(CalculateTotalIngredients);
            ClearQuantitiesCommand = new CommandRunner(ClearRecipes, CanClearRecipes);
            CopyIngredientsCommand = new CommandRunner(CopyIngredientsToClipboard, CanCopyIngredients);
            SaveRecipeFavoritesCommand = new CommandRunner(SaveRecipes, CanSaveRecipes);
            dialogCoordinator = instance;
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
