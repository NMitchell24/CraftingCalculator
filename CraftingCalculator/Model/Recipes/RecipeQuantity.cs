using CraftingCalculator.Model.Ingredients;
using System.ComponentModel;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// A class that represents a recipe and quantity
    /// </summary>
    public class RecipeQuantity : INotifyPropertyChanged
    {
        public Recipe Recipe { get; set; }
        public int Quantity { get; set; }
        public string CoreComponents { get => Recipe.GetCoreComponents(Quantity); private set { } }
        public string Name { get => Recipe.Name; private set { } }
        public string Type { get => Recipe.Type; private set { } }
        public string Tooltip
        {
            get
            {
                if(Recipe.GetType().IsSubclassOf(typeof(ComplexRecipe)))
                {
                    return ((ComplexRecipe)Recipe).Tooltip;
                }
                else
                {
                    return Recipe.Tooltip;
                }
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}
