using CraftingCalculator.ViewModel.Ingredients;
using System;

namespace CraftingCalculator.ViewModel.Recipes
{
    /// <summary>
    /// A class that represents a recipe and quantity
    /// </summary>
    public class RecipeQuantity : AbstractPropertyChanged
    {
        public Recipe Recipe { get; set; }
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                // This should match the maximum set for the NumericUpDown control in the RecipesView.xaml
                // Prevents the user from increasing the ingredient totals over the maximum for the numeric up down.
                if(value <= 500)
                {
                    _quantity = Math.Abs(value);
                }
            }
        }
        public string Name { get => Recipe.Name; private set { } }
        public string Type { get => Recipe.FilterType; private set { } }
        public string Tooltip
        {
            get
            {
                return Recipe.Tooltip;
            }

            private set { }
        }
        public IngredientMap Ingredients { get => Recipe.GetIngredients(); private set { } }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        
        //public bool IsSelected { get => Recipe.IsSelected; set => Recipe.IsSelected = value; }

        public RecipeQuantity(Recipe recipe, int quantity)
        {
            Recipe = recipe;
            Quantity = quantity;
        }
    }
}
