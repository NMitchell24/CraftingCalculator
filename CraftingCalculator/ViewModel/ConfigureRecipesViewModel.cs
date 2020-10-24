using CraftingCalculator.Utilities;
using CraftingCalculator.Service;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftingCalculator.ViewModel.Recipes;
using CraftingCalculator.ViewModel.Ingredients;

namespace CraftingCalculator.ViewModel
{
    class ConfigureRecipesViewModel : AbstractPropertyChanged
    {
        // Class level Variables used by multiple methods.
        private IDialogCoordinator dialogCoordinator;
        public IReadOnlyList<DataType> DataTypeList { get; }
        public int SwitchView { get; set; }
        public IBaseDataRecord ItemForUpdate { get; set; }   
        public bool ShowProgressRing { get; set; }
        public bool EnableDisableWindow { get; set; }

        public ObservableCollection<IBaseDataRecord> DataRecords { get; set; }

        public ConfigureRecipesViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
            DataTypeList = DataTypeUtil.GetDataTypeList().ToArray();
            SelectedType = DataTypeList[0];
            SwitchView = 0;
            RaisePropertyChanged(nameof(DataRecords));

            SaveItemCommand = new CommandRunner(SaveItem);
            ResetItemCommand = new CommandRunner(ResetItem);
            AddNewItemCommand = new CommandRunner(AddItem);
            DeleteItemCommand = new CommandRunner(DeleteItem);
            CopyItemCommand = new CommandRunner(CopyItem);

            ShowProgressRing = false;
            EnableDisableWindow = true;
        }

        public CommandRunner SaveItemCommand { get; set; }
        private void SaveItem(object obj)
        {
            string name = ItemForUpdate.Name;
            if (ItemForUpdate.Type == DataType.Ingredient)
            {
                IngredientService.SaveIngredient((Ingredient)ItemForUpdate);
            }
            else if (ItemForUpdate.Type == DataType.RecipeType)
            {
                RecipeFilterService.SaveRecipeFilter((RecipeFilter)ItemForUpdate);
            }
            else if (ItemForUpdate.Type == DataType.Recipe)
            {
                //TODO: Do the thing that does other stuff
            }

            SelectedType = ItemForUpdate.Type;
            IBaseDataRecord rec = DataRecords.Where(x => x.Name == name).FirstOrDefault();
            SelectedItem = rec;
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

            RaisePropertyChanged(nameof(ItemForUpdate));
        }

        public CommandRunner AddNewItemCommand { get; set; }
        private void AddItem(object obj)
        {
            SelectedItem = SelectedType.GetDataRecord();
            SelectedType = ItemForUpdate.Type;

            RaisePropertyChanged(nameof(SelectedItem));
            RaisePropertyChanged(nameof(DataRecords));
            RaisePropertyChanged(nameof(ItemForUpdate));
        }

        public CommandRunner DeleteItemCommand { get; set; }
        private async void DeleteItem(object obj)
        {
            bool doDelete = false;

            var settings = new MetroDialogSettings { AffirmativeButtonText = "Delete", NegativeButtonText = "Cancel" };
            var yesNo = await dialogCoordinator.ShowMessageAsync(this, "Delete " + SelectedType.GetDescription(),
            "This " + SelectedType.GetDescription() + " will be deleted forever and removed from any recipes " +
            (SelectedType == DataType.Recipe ? " or recipe favorites " : "") +
            "where it is used.  " +
            "Are you sure you would like to continue?",
            MessageDialogStyle.AffirmativeAndNegative,
            settings);

            doDelete = yesNo == MessageDialogResult.Affirmative;

            if (doDelete)
            {
                ShowHideProgress();
                await Task.Factory.StartNew(() =>
                {
                    if (ItemForUpdate.Type == DataType.Ingredient)
                    {
                        IngredientService.DeleteIngredient((Ingredient)ItemForUpdate);
                    }
                    else if (ItemForUpdate.Type == DataType.RecipeType)
                    {
                        RecipeFilterService.DeleteRecipeFilter((RecipeFilter)ItemForUpdate);
                    }
                    else if (ItemForUpdate.Type == DataType.Recipe)
                    {
                        //TODO: Do the thing that does other stuff
                    }
                    ItemForUpdate = SelectedType.GetDataRecord();
                    SelectedType = ItemForUpdate.Type;

                    RaisePropertyChanged(nameof(ItemForUpdate));
                    RaisePropertyChanged(nameof(DataRecords));
                    RaisePropertyChanged(nameof(SelectedItem));
                }).ContinueWith(Task =>
                {
                    ShowHideProgress();
                });              
            }
        }

        public CommandRunner CopyItemCommand { get; set; }
        private void CopyItem(object obj)
        {
            IBaseDataRecord record = ItemForUpdate.CopyForSave();
            SelectedItem = record.Clone();
            SelectedType = record.Type;
            ItemForUpdate = record;

            RaisePropertyChanged(nameof(SelectedItem));
            RaisePropertyChanged(nameof(DataRecords));
            RaisePropertyChanged(nameof(ItemForUpdate));
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
                    DataRecords = new ObservableCollection<IBaseDataRecord>(IngredientService.GetAllIngredients());
                    SwitchView = 0;
                }
                else if(_selectedType == DataType.RecipeType)
                {
                    //Need to remove All from the list so that it cannot be edited.
                    List<RecipeFilter> filterList = RecipeFilterService.GetRecipeFilters();
                    filterList.RemoveAt(0);
                    DataRecords = new ObservableCollection<IBaseDataRecord>(filterList);
                    SwitchView = 1;
                }
                else if (_selectedType == DataType.Recipe)
                {
                    DataRecords = new ObservableCollection<IBaseDataRecord>(RecipeService.GetAllRecipes());
                    SwitchView = 2;
                }

                ItemForUpdate = SelectedType.GetDataRecord();

                RaisePropertyChanged(nameof(DataRecords));
                RaisePropertyChanged(nameof(SwitchView));
            }
        }

        private IBaseDataRecord _selectedItem;
        public IBaseDataRecord SelectedItem
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
    }
}
