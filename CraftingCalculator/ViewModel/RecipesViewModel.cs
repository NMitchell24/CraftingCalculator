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
            RecipeFilters = RecipeUtil.GetRecipeFilters();
            SelectedFilter = RecipeFilters[0];
            RecipesList = new ObservableCollection<Recipe>(RecipeUtil.GetRecipesByFilter(SelectedFilter));
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
