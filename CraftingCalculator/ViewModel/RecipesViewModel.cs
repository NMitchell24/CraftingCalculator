using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CraftingCalculator.Utilities;
using CraftingCalculator.Model.Recipes;

namespace CraftingCalculator.ViewModel
{
    class RecipesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Recipe> RecipesList { get; set; }
        public List<RecipeFilter> RecipeFilters { get; set; }

        private RecipeFilter _selectedFilter;
        public RecipeFilter SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter == value)
                    return;

                _selectedFilter = value;

                ReloadRecipesForFilter(_selectedFilter);
            }
        }

        public RecipesViewModel()
        {
            RecipesList = new ObservableCollection<Recipe>(RecipeUtil.GetRecipesByType(RecipeType.ALL));
            RecipeFilters = RecipeUtil.GetRecipeFilters();
            SelectedFilter = RecipeFilters[0];
        }

        public void ReloadRecipesForFilter(RecipeFilter filter)
        {
            RecipesList = new ObservableCollection<Recipe>(RecipeUtil.GetRecipesByType(filter.Type));
            RaisePropertyChanged(nameof(RecipesList));
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
