using CraftingCalculator.ViewModel.Ingredients;
using CraftingCalculator.ViewModel.Recipes;
using CraftingCalculator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using CraftingCalculator.Service;

namespace CraftingCalculator.ViewModel
{
    public class RecipesViewModel : AbstractPropertyChanged
    {
        #region class-definition
        // Class level Variables used by multiple methods.
        private IDialogCoordinator dialogCoordinator;
        private RecipeMap _recipeMap = new RecipeMap();
        private IngredientMap _ingredientMap = new IngredientMap();

        // Class level Collections used by multiple methods.
        public ObservableCollection<Recipe> RecipesList { get; set; }
        public List<RecipeFilter> RecipeFilters { get; set; }
        public ObservableCollection<RecipeQuantity> RecipeQuantities { get; set; }
        public ObservableCollection<IngredientQuantity> TotalIngredients { get; set; }
        public ObservableCollection<RecipeTree> RecipeTotals { get; set; }
        public ObservableCollection<RecipeFavorite> RecipeFavorites { get; set; }

        private const string _numberFormat = "{0:C2}";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance"></param>
        public RecipesViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            AddRecipeCommand = new CommandRunner(AddRecipes, CanAddRecipes);
            RemoveRecipeCommand = new CommandRunner(RemoveRecipes, CanRemoveRecipes);
            RecalculateTotalsCommand = new CommandRunner(CalculateTotalIngredients);
            ClearQuantitiesCommand = new CommandRunner(ClearRecipes, IsRecipesExist);
            CopyIngredientsCommand = new CommandRunner(CopyIngredientsToClipboard, CanCopyIngredients);
            SaveRecipeFavoritesCommand = new CommandRunner(SaveRecipes, IsRecipesExist);
            DeleteFavoriteCommand = new CommandRunner(DeleteSelectedFavorite, CanDeleteFavorite);
            ExpandAllCommand = new CommandRunner(ExpandAllRecipes, CanExpandCollapseAll);
            CollapseAllCommand = new CommandRunner(CollapseAllRecipes, CanExpandCollapseAll);
            
            RecipeFilters = RecipeFilterService.GetRecipeFilters();
            SelectedFilter = RecipeFilters[0];

            RecipesList = new ObservableCollection<Recipe>(RecipeService.GetRecipesByFilter(SelectedFilter));
            RecipeFavorites = new ObservableCollection<RecipeFavorite>(RecipeFavoriteService.GetAllRecipeFavorites());
            RecipeTotals = new ObservableCollection<RecipeTree>();        
        }
        #endregion

        #region commands
        // Add Recipe Command and methods.
        public CommandRunner AddRecipeCommand { get; set; }
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
        }

        // Remove Recipe Command and Methods.
        public CommandRunner RemoveRecipeCommand { get; set; }
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
        }

        // Recalculate Totals Command and Methods
        public CommandRunner RecalculateTotalsCommand { get; set; }
        private void CalculateTotalIngredients(object obj = null)
        {
            _totalCost = 0;
            _totalValue = 0;
            _ingredientMap.Reset();
            foreach (RecipeQuantity q in RecipeQuantities)
            {
                foreach (IngredientQuantity i in q.Ingredients.IngredientList)
                {
                    _ingredientMap.Add(i.Ingredient, (i.Quantity * q.Quantity));
                    _totalCost += i.TotalCost * q.Quantity;
                }
                _totalValue += q.TotalValue;
            }
            TotalIngredients = new ObservableCollection<IngredientQuantity>(_ingredientMap.IngredientList.OrderBy(x => x.Name));

            // rebuild recipe tree
            CalculateRecipeTotals();
        }

        // Clear Quantities command and Methods.
        public CommandRunner ClearQuantitiesCommand { get; set; }
        private void ClearRecipes(object obj)
        {
            _recipeMap.Reset();
            RecipeQuantities.Clear();

            SelectedFav = null;

            CalculateTotalIngredients();
        }

        // Copy Ingredients Command and Methods.
        public CommandRunner CopyIngredientsCommand { get; set; }
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

        // Save Recipe Favorites Command and Methods.
        public CommandRunner SaveRecipeFavoritesCommand { get; set; }
        private async void SaveRecipes(object obj)
        {
            bool doSaveExisting = false;
            bool doSave = true;
            var name = "";
            if (SelectedFav != null)
            {
                var settings = new MetroDialogSettings { AffirmativeButtonText = "Update", NegativeButtonText = "Create New" };
                var yesNo = await dialogCoordinator.ShowMessageAsync(this, "Update or Create New?",
                "Would you like to update '" + SelectedFav.Name + "' or create a new favorite?",
                MessageDialogStyle.AffirmativeAndNegative,
                settings);

                doSaveExisting = yesNo == MessageDialogResult.Affirmative;

                if (doSaveExisting)
                    name = SelectedFav.Name;
            }

            if (((!doSaveExisting) && SelectedFav != null) || SelectedFav == null)
            {
                name = await dialogCoordinator
                    .ShowInputAsync(this, "Save Recipe Favorites", "Enter a Name for this Favorite:");

                //User cancelled.
                if (name == null)
                    return;

                if (RecipeFavoriteService.DoesFavoriteExist(name))
                {
                    var settings = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" };
                    var yesNo = await dialogCoordinator.ShowMessageAsync(this, "Confirm",
                    "Recipe Favorite already exists, do you want to overwrite?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    settings);

                    doSave = yesNo == MessageDialogResult.Affirmative;
                }
            }
            if (doSave || doSaveExisting)
            {
                RecipeFavorite fav = new RecipeFavorite()
                {
                    Name = name
                };
                RecipeFavoriteService.SaveRecipeFavorite(fav, RecipeQuantities.ToList());
            }

            RecipeFavorites = new ObservableCollection<RecipeFavorite>(RecipeFavoriteService.GetAllRecipeFavorites());
            SelectedFav = RecipeFavorites.Where(x => x.Name == name).FirstOrDefault();

            RaiseChanged();
        }

        //Delete Recipe Favorite Command and Methods.
        public CommandRunner DeleteFavoriteCommand { get; set; }
        private bool CanDeleteFavorite()
        {
            return SelectedFav != null;
        }

        private async void DeleteSelectedFavorite(object obj)
        {
            bool doDelete = false;

            var settings = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" };
            var yesNo = await dialogCoordinator.ShowMessageAsync(this, "Confirm",
            "This action will permanently delete the saved favorites '" + SelectedFav.Name + "'.  Are you sure you want to continue?",
            MessageDialogStyle.AffirmativeAndNegative,
            settings);

            doDelete = yesNo == MessageDialogResult.Affirmative;

            if (doDelete)
            {
                RecipeFavoriteService.DeleteFavoriteData(SelectedFav);
                SelectedFav = null;
                RecipeFavorites = new ObservableCollection<RecipeFavorite>(RecipeFavoriteService.GetAllRecipeFavorites());
                RaiseChanged();
            }
        }

        //Expand and Collapse All Command and methods
        public CommandRunner ExpandAllCommand { get; set; }
        public CommandRunner CollapseAllCommand { get; set; }
        private bool CanExpandCollapseAll()
        {
            return RecipeTotals != null && RecipeTotals.Count() > 0;
        }

        private void ExpandAllRecipes(object obj)
        {
            ExpandCollapseAll(true);
        }
        
        private void CollapseAllRecipes(object obj)
        {
            ExpandCollapseAll(false);
        }

        private void ExpandCollapseAll(bool isExpand)
        {
            List<RecipeTree> temp = new List<RecipeTree>();
            foreach (RecipeTree tree in RecipeTotals)
            {
                tree.ExpandCollapseAll(isExpand);
                temp.Add(tree);
            }

            RecipeTotals = new ObservableCollection<RecipeTree>(temp);
            RaiseChanged();
        }

        #endregion

        #region selected-items
        // Favorites
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
                    List<RecipeQuantity> quans = RecipeFavoriteService.GetRecipeQuantitiesForFavorite(_selectedFav);
                    foreach (RecipeQuantity q in quans)
                    {
                        _recipeMap.Add(q.Recipe, q.Quantity);
                    }

                    RecipeQuantities = new ObservableCollection<RecipeQuantity>(_recipeMap.RecipeList);

                    CalculateTotalIngredients();
                }
            }
        }

        // Filters
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

        // Recipes
        private Recipe _selectedRecipe;
        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                var selectedItems = RecipesList.Where(x => x.IsSelected).Count();
                RaiseChanged();
            }
        }
        
        private int _selectedRecipeIndex;
        public int SelectedRecipeIndex
        {
            get => _selectedRecipeIndex;
            set
            {
                _selectedRecipeIndex = value;
                RaiseChanged();
            }
        }

        // Recipe Quantities
        private RecipeQuantity _selectedRecipeQuantity;
        public RecipeQuantity SelectedRecipeQuantity
        {
            get => _selectedRecipeQuantity;
            set
            {
                var selectedItems = RecipeQuantities.Where(x => x.IsSelected).Count();
                RaiseChanged();
            }
        }

        private int _selectedRecipeQuantityIndex;
        public int SelectedRecipeQuantityIndex
        {
            get => _selectedRecipeQuantityIndex;
            set
            {
                _selectedRecipeQuantityIndex = value;
                RaiseChanged();
            }
        }

        //Cost and Values
        private double _totalCost;
        public string TotalCost
        {
            get => string.Format(_numberFormat, _totalCost);
            private set { }
        }

        private double _totalValue;
        public string TotalValue
        {
            get => string.Format(_numberFormat, _totalValue);
            private set { }
        }

        public string PotentialProfit
        {
            get => string.Format(_numberFormat, (_totalValue - _totalCost));
            private set { }
        }

        #endregion

        #region helper-methods
        /// <summary>
        /// Helper Method used by multiple commands.
        /// </summary>
        /// <returns></returns>
        private bool IsRecipesExist()
        {
            return RecipeQuantities != null && RecipeQuantities.Count() > 0;
        }

        /// <summary>
        /// Helper Method used by multiple commands.
        /// </summary>
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

            RaiseChanged();
        }

        /// <summary>
        /// Helper method to reload recipes list when Filter changes.
        /// </summary>
        /// <param name="filter"></param>
        public void ReloadRecipesForFilter(RecipeFilter filter)
        {
            RecipesList = new ObservableCollection<Recipe>(RecipeService.GetRecipesByFilter(filter));
            RaiseChanged();
        }

        /// <summary>
        /// Raises changed for all lists, selected values, and commands.
        /// </summary>
        private void RaiseChanged()
        {
            RaisePropertyChanged(nameof(SelectedFilter));
            RaisePropertyChanged(nameof(SelectedRecipe));
            RaisePropertyChanged(nameof(SelectedRecipeQuantity));
            RaisePropertyChanged(nameof(SelectedFav));
            RaisePropertyChanged(nameof(RecipesList));
            RaisePropertyChanged(nameof(RecipeFilters));
            RaisePropertyChanged(nameof(RecipeQuantities));
            RaisePropertyChanged(nameof(TotalIngredients));
            RaisePropertyChanged(nameof(RecipeTotals));
            RaisePropertyChanged(nameof(RecipeFavorites));
            RaisePropertyChanged(nameof(TotalCost));
            RaisePropertyChanged(nameof(TotalValue));
            RaisePropertyChanged(nameof(PotentialProfit));
            AddRecipeCommand.RaiseCanExecuteChanged();
            RemoveRecipeCommand.RaiseCanExecuteChanged();
            RecalculateTotalsCommand.RaiseCanExecuteChanged();
            ClearQuantitiesCommand.RaiseCanExecuteChanged();
            CopyIngredientsCommand.RaiseCanExecuteChanged();
            SaveRecipeFavoritesCommand.RaiseCanExecuteChanged();
            DeleteFavoriteCommand.RaiseCanExecuteChanged();
            ExpandAllCommand.RaiseCanExecuteChanged();
            CollapseAllCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
