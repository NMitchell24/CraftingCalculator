using CraftingCalculator.Model.Ingredients;
using System.ComponentModel;
using System.Text;
using System;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// Abstract Recipe Implementation for recipes that contain only basic Elements as ingredients.
    /// When Adding new Recipes they will show up automagically as long as they are added to an existing Namespace.
    /// If creating a new Namespace then you must add a RecipeType as well, 
    /// and then create a filter for that type in RecipeUtil.
    /// </summary>
    public abstract class Recipe : INotifyPropertyChanged
    {

        protected IngredientMap Ingredients = new IngredientMap();
        public string Name { get; protected set; }
        public string Type { get; protected set; }
        public string Tooltip
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Name);
                sb.AppendLine(Type);
                sb.Append(Environment.NewLine);
                sb.AppendLine("Ingredients:");

                foreach (IngredientQuantity ingredient in Ingredients.IngredientList)
                {
                    sb.AppendLine(ingredient.Name + " x" + ingredient.Quantity);
                }

                return sb.ToString();
            }
            protected set { Tooltip = value;}
        }
       

        public virtual IngredientMap GetIngredients()
        {
            return Ingredients;
        }

        public IngredientMap GetBaseIngredients()
        {
            return Ingredients;
        }

       public virtual RecipeTree GetRecipeNodes( int quantity )
        {
            RecipeTree ret = new RecipeTree
            {
                Name = Name + " x" + quantity
            };

            foreach (IngredientQuantity i in Ingredients.IngredientList)
            {
                ret.AddRecipeNode(new RecipeTree(i.Name + " x" + (i.Quantity * quantity)));
            }

            return ret;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}
