using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Utils;
using CraftingCalculator.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.ViewModel
{
    class ConfigureRecipesViewModel : AbstractPropertyChanged
    {
        // Class level Variables used by multiple methods.
        private IDialogCoordinator dialogCoordinator;
        private readonly IIngredientService _ingredientService;
        private readonly IRecipeFilterService _recipeFilterService;
        private readonly IRecipeService _recipeService;
        private readonly IDatabaseAdminService _databaseAdminService;
        public IReadOnlyList<DataType> DataTypeList { get; }
        public int SwitchView { get; set; }
        public IBaseDataRecord? ItemForUpdate { get; set; }
        public bool ShowProgressRing { get; set; }
        public bool EnableDisableWindow { get; set; }

        public ObservableCollection<IBaseDataRecord>? DataRecords { get; set; }

        public ConfigureRecipesViewModel(
            IDialogCoordinator instance,
            IIngredientService ingredientService,
            IRecipeFilterService recipeFilterService,
            IRecipeService recipeService,
            IDatabaseAdminService databaseAdminService)
        {
            dialogCoordinator = instance;
            _ingredientService = ingredientService;
            _recipeFilterService = recipeFilterService;
            _recipeService = recipeService;
            _databaseAdminService = databaseAdminService;
            DataTypeList = DataTypeUtil.GetDataTypeList().ToArray();
            SelectedType = DataTypeList[0];
            RecipeSubTypes = new List<DataType>()
            {
                DataType.Ingredient,
                DataType.Recipe
            };
            RecipeSelectedType = RecipeSubTypes[0];
            SwitchView = 0;
            RaisePropertyChanged(nameof(DataRecords));

            SaveItemCommand = new CommandRunner(SaveItem);
            ResetItemCommand = new CommandRunner(ResetItem);
            AddNewItemCommand = new CommandRunner(AddItem);
            DeleteItemCommand = new CommandRunner(DeleteItem);
            DeleteAllCommand = new CommandRunner(DeleteAllData);
            CopyItemCommand = new CommandRunner(CopyItem);
            AddRecipeValues = new CommandRunner(AddToRecipe);
            DeleteRecipeIngredient = new CommandRunner(DeleteIngredient);

            ShowProgressRing = false;
            EnableDisableWindow = true;
        }

        public CommandRunner SaveItemCommand { get; set; }
        private void SaveItem(object obj)
        {
            if (ItemForUpdate != null)
            {
                string name = ItemForUpdate.Name ?? "";
                if (ItemForUpdate.Type == DataType.Ingredient)
                {
                    _ingredientService.SaveIngredientAsync(ItemForUpdate as Ingredient).GetAwaiter().GetResult();
                }
                else if (ItemForUpdate.Type == DataType.RecipeFilter)
                {
                    _recipeFilterService.SaveRecipeFilterAsync(ItemForUpdate as RecipeFilter).GetAwaiter().GetResult();
                }
                else if (ItemForUpdate.Type == DataType.Recipe)
                {
                    _recipeService.SaveRecipeAsync(ItemForUpdate as Recipe).GetAwaiter().GetResult();
                }

                SelectedType = ItemForUpdate.Type;
                IBaseDataRecord rec = DataRecords.Where(x => x.Name == name).FirstOrDefault();
                SelectedItem = rec;
            }
        }

        public CommandRunner ResetItemCommand { get; set; }
        private void ResetItem(object obj)
        {
            if(SelectedItem != null)
            {
                ItemForUpdate = SelectedItem.Clone();
            }
            else
            {
                ItemForUpdate = SelectedType.GetDataRecord();
            }

            if(SelectedType == DataType.Recipe)
            {
                ResetRecipeValues();
            }

            RaisePropertyChanged(nameof(ItemForUpdate));
        }

        public CommandRunner AddNewItemCommand { get; set; }
        private void AddItem(object obj)
        {
            if (ItemForUpdate != null)
            {
                SelectedItem = SelectedType.GetDataRecord();
                SelectedType = ItemForUpdate.Type;

                RaisePropertyChanged(nameof(SelectedItem));
                RaisePropertyChanged(nameof(DataRecords));
                RaisePropertyChanged(nameof(ItemForUpdate));
            }
        }

        public CommandRunner DeleteItemCommand { get; set; }
        private async void DeleteItem(object obj)
        {
            if (ItemForUpdate != null)
            {
                bool doDelete = false;

                var settings = new MetroDialogSettings { AffirmativeButtonText = "Delete", NegativeButtonText = "Cancel" };
                var yesNo = await dialogCoordinator.ShowMessageAsync(this, "Delete " + SelectedType.GetDescription(),
                "This " + SelectedType.GetDescription() + " will be deleted forever and removed from any recipes " +
                (SelectedType == DataType.Recipe ? "or recipe favorites " : "") +
                "where it is used.  " +
                Environment.NewLine + Environment.NewLine +
                "Are you sure you would like to continue?",
                MessageDialogStyle.AffirmativeAndNegative,
                settings);

                doDelete = yesNo == MessageDialogResult.Affirmative;

                if (doDelete)
                {
                    ShowHideProgress();

                    if (ItemForUpdate.Type == DataType.Ingredient)
                    {
                        await _ingredientService.DeleteIngredientAsync(ItemForUpdate as Ingredient);
                    }
                    else if (ItemForUpdate.Type == DataType.RecipeFilter)
                    {
                        await _recipeFilterService.DeleteRecipeFilterAsync(ItemForUpdate as RecipeFilter);
                    }
                    else if (ItemForUpdate.Type == DataType.Recipe)
                    {
                        await _recipeService.DeleteRecipeAsync(ItemForUpdate as Recipe);
                    }
                    ItemForUpdate = SelectedType.GetDataRecord();
                    SelectedType = ItemForUpdate.Type;

                    RaisePropertyChanged(nameof(ItemForUpdate));
                    RaisePropertyChanged(nameof(DataRecords));
                    RaisePropertyChanged(nameof(SelectedItem));

                    ShowHideProgress();
                }
            }
        }

        public CommandRunner DeleteAllCommand { get; set; }
        private async void DeleteAllData(object obj)
        {
            bool doDelete = false;

            var settings = new MetroDialogSettings { AffirmativeButtonText = "Delete", NegativeButtonText = "Cancel" };
            var yesNo = await dialogCoordinator.ShowMessageAsync(this, "Delete Everything?",
            "This will completely remove all data from the application.  Including all Favorites, Recipes, Recipe Filters, and Ingredients.  " +
            "It is recommended that you make a backup of your CraftingCalculator.db file before deleting." +
            Environment.NewLine + Environment.NewLine +
            "Are you sure you would like to continue?",
            MessageDialogStyle.AffirmativeAndNegative,
            settings);

            doDelete = yesNo == MessageDialogResult.Affirmative;

            if(doDelete)
            {
                ShowHideProgress();

                await _databaseAdminService.DeleteAllDataAsync();

                ItemForUpdate = SelectedType.GetDataRecord();
                SelectedType = ItemForUpdate.Type;

                RaisePropertyChanged(nameof(ItemForUpdate));
                RaisePropertyChanged(nameof(DataRecords));
                RaisePropertyChanged(nameof(SelectedItem));

                ShowHideProgress();
            }
        }

        public CommandRunner CopyItemCommand { get; set; }
        private void CopyItem(object obj)
        {
            if (ItemForUpdate != null)
            {
                IBaseDataRecord record = ItemForUpdate.CopyForSave();
                SelectedType = record.Type;
                SelectedItem = record.Clone();

                RaisePropertyChanged(nameof(DataRecords));
            }
        }

        private DataType _selectedType;
        public DataType SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                if(_selectedType == DataType.Ingredient)
                {
                    DataRecords = new ObservableCollection<IBaseDataRecord>(_ingredientService.GetAllIngredientsAsync().GetAwaiter().GetResult());
                    SwitchView = 0;
                }
                else if(_selectedType == DataType.RecipeFilter)
                {
                    //Need to remove All from the list so that it cannot be edited.
                    List<RecipeFilter> filterList = _recipeFilterService.GetRecipeFiltersAsync().GetAwaiter().GetResult();
                    filterList.RemoveAt(0);
                    DataRecords = new ObservableCollection<IBaseDataRecord>(filterList);
                    SwitchView = 1;
                }
                else if (_selectedType == DataType.Recipe)
                {
                    DataRecords = new ObservableCollection<IBaseDataRecord>(_recipeService.GetAllRecipesAsync().GetAwaiter().GetResult());
                    InitializeRecipeValues();
                    SwitchView = 2;
                }

                SelectedItem = SelectedType.GetDataRecord();

                RaisePropertyChanged(nameof(DataRecords));
                RaisePropertyChanged(nameof(SwitchView));
            }
        }

        private IBaseDataRecord? _selectedItem;
        public IBaseDataRecord? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value != null)
                {
                    _selectedItem = value;
                    ItemForUpdate = value.Clone();
                    RaisePropertyChanged(nameof(SelectedItem));
                    RaisePropertyChanged(nameof(ItemForUpdate));

                    // Reload Recipe specific Data.
                    if(ItemForUpdate.Type == DataType.Recipe)
                    {
                        ResetRecipeValues();
                    }
                }
            }
        }

        private int _selectedItemIndex;
        public int SelectedItemIndex
        {
            get => _selectedItemIndex;
            set
            {
                _selectedItemIndex = value;
            }
        }

        public void ShowHideProgress()
        {
            EnableDisableWindow = !EnableDisableWindow;
            ShowProgressRing = !ShowProgressRing;
            RaisePropertyChanged(nameof(ShowProgressRing));
            RaisePropertyChanged(nameof(EnableDisableWindow));
        }

        #region recipeSpecificLogic

        private void InitializeRecipeValues()
        {
            RecipeFilters = _recipeFilterService.GetRecipeFiltersAsync().GetAwaiter().GetResult();
            //Remove ALL from the list as it is not a valid filter to set on Recipes
            RecipeFilters.RemoveAt(0);
            RaisePropertyChanged(nameof(RecipeFilters));

            if (ItemForUpdate?.Id > 0)
            {
                UpdateSelectedFilter();
            }
        }

        private void ResetRecipeValues()
        {
            UpdateSelectedFilter();
            ResetQuantity();
            UpdateRecipeQuantityValues();
            //Reset to default data type selection to prevent data from being out of sync.
            RecipeSelectedType = DataType.Ingredient;
            RaisePropertyChanged(nameof(RecipeSelectedType));
        }

        private void ResetQuantity()
        {
            QuantityToAdd = 0;
            RaisePropertyChanged(nameof(QuantityToAdd));
        }

        private void UpdateSelectedFilter()
        {
            if (ItemForUpdate?.Type == DataType.Recipe)
            {
                //Make sure selected Recipe Filter matches from Recipe
                Recipe? recipe = ItemForUpdate as Recipe;
                if (recipe?.Filter != null)
                {
                    SelectedFilter = RecipeFilters.FirstOrDefault(x => x.Name == recipe.Filter.Name);
                    RaisePropertyChanged(nameof(SelectedFilter));
                }
            }
        }

        public List<RecipeFilter>? RecipeFilters { get; set; }

        private RecipeFilter? _selectedFilter;
        public RecipeFilter? SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                if(_selectedFilter != null)
                {
                    if (ItemForUpdate is Recipe recipe)
                    {
                        recipe.Filter = _selectedFilter;
                    }
                }
            }
        }

        public IReadOnlyList<DataType> RecipeSubTypes { get; }
        private DataType _recipeSelectedType;
        public DataType RecipeSelectedType
        {
            get => _recipeSelectedType;
            set
            {
                _recipeSelectedType = value;
                if(_recipeSelectedType == DataType.Ingredient)
                {
                    RecipeIngredientValues = new ObservableCollection<IBaseDataRecord>(_ingredientService.GetAllIngredientsAsync().GetAwaiter().GetResult());
                }
                else if (_recipeSelectedType == DataType.Recipe)
                {
                    RecipeIngredientValues = new ObservableCollection<IBaseDataRecord>(_recipeService.GetAllRecipesAsync().GetAwaiter().GetResult());
                    // Remove the current item that's being edited to prevent an infinite loop scenario when calculating ingredients.
                    IBaseDataRecord current = RecipeIngredientValues.FirstOrDefault(x => x.Id == ItemForUpdate?.Id);
                    RecipeIngredientValues.Remove(current);
                }
                ResetQuantity();
                RaisePropertyChanged(nameof(RecipeIngredientValues));
            }
        }

        public ObservableCollection<IBaseDataRecord>? RecipeIngredientValues { get; set; }
        private IBaseDataRecord? _recipeSelectedIngredientValue;
        public IBaseDataRecord? RecipeSelectedIngredientValue
        {
            get => _recipeSelectedIngredientValue;
            set
            {
                _recipeSelectedIngredientValue = value;
            }
        }

        private int _quantityToAdd;
        public int QuantityToAdd
        {
            get => _quantityToAdd;
            set
            {
                _quantityToAdd = value;
            }
        }

        public CommandRunner AddRecipeValues { get; set; }
        private void AddToRecipe(object obj)
        {
            if (ItemForUpdate is Recipe recipe && RecipeSelectedIngredientValue != null)
            {
                if (RecipeSelectedType == DataType.Ingredient)
                {
                    recipe.Ingredients.Add(RecipeSelectedIngredientValue as Ingredient, QuantityToAdd);
                }
                else if (RecipeSelectedType == DataType.Recipe)
                {
                    recipe.ChildRecipes.Add(RecipeSelectedIngredientValue as Recipe, QuantityToAdd);
                }
            }
            //Update the Recipe Quantity Values.
            UpdateRecipeQuantityValues();
        }

        public CommandRunner DeleteRecipeIngredient { get; set; }
        public void DeleteIngredient(object obj)
        {
            IBaseQuantityRecord? quantity = SelectedRecipeQuantity;
            Recipe? recipe = ItemForUpdate as Recipe;
            if (obj is IBaseQuantityRecord q)
            {
                quantity = q;
            }

            if (quantity?.Type == DataType.Ingredient)
            {
                if (recipe != null && quantity is IngredientQuantity ingQuantity)
                {
                    recipe.Ingredients.Remove(ingQuantity.Ingredient, quantity.Quantity);
                }
            }
            else if (quantity?.Type == DataType.Recipe)
            {
                if (recipe != null && quantity is RecipeQuantity recQuantity)
                {
                    recipe.ChildRecipes.Remove(recQuantity.Recipe, recQuantity.Quantity);
                }
            }

            UpdateRecipeQuantityValues();
        }

        public ObservableCollection<IBaseQuantityRecord>? RecipeQuantityValues { get; set; }
        private IBaseQuantityRecord? _selectedRecipeQuantity;
        public IBaseQuantityRecord? SelectedRecipeQuantity
        {
            get => _selectedRecipeQuantity;
            set
            {
                _selectedRecipeQuantity = value;
            }
        }
        private int _selectedRecipeQuantityIndex;
        public int SelectedRecipeQuantityIndex
        {
            get => _selectedRecipeQuantityIndex;
            set
            {
                _selectedRecipeQuantityIndex = value;
            }
        }

        private void UpdateRecipeQuantityValues()
        {
            if(ItemForUpdate?.Type == DataType.Recipe)
            {
                if (ItemForUpdate is Recipe recipe)
                {
                    List<IngredientQuantity> ingredients = recipe.Ingredients.IngredientList.ToList();
                    List<IBaseQuantityRecord> combinedList = new List<IBaseQuantityRecord>();
                    combinedList.AddRange(ingredients);

                    if (recipe.ChildRecipes != null)
                    {
                        List<RecipeQuantity> childRecipes = recipe.ChildRecipes.RecipeList.ToList();
                        combinedList.AddRange(childRecipes);
                    }

                    RecipeQuantityValues = new ObservableCollection<IBaseQuantityRecord>(combinedList.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList());

                    RaisePropertyChanged(nameof(RecipeQuantityValues));
                    RaisePropertyChanged(nameof(SelectedRecipeQuantity));
                }
            }
        }

        #endregion
    }
}
