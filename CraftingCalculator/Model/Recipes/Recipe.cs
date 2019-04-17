using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Utilities;
using System.ComponentModel;
using System.Text;
using System;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// Represents an individual UI Model for the Recipes
    /// </summary>
    public class Recipe : INotifyPropertyChanged
    {
        public IngredientMap Ingredients { get; set; }
        public RecipeMap ChildRecipes { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
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

                if(ChildRecipes != null)
                {
                    foreach (RecipeQuantity recipe in ChildRecipes.RecipeList)
                    {
                        sb.AppendLine(recipe.Name + " x" + recipe.Quantity);
                    }
                }

                return sb.ToString();
            }
            set { Tooltip = value;}
        }

        public IngredientMap GetIngredients()
        {
            IngredientMap NewIngredients = new IngredientMap(Ingredients);

            if(ChildRecipes != null)
            {
                foreach (RecipeQuantity recipe in ChildRecipes.RecipeList)
                {
                    IngredientMap RecipeIngredients = new IngredientMap(recipe.Ingredients);

                    NewIngredients = IngredientUtil.CombineIngredients(RecipeIngredients, NewIngredients, recipe.Quantity);
                }
            }

            return NewIngredients;
        }

  
        public RecipeTree GetRecipeNodes(int quantity)
        {
            RecipeTree ret = new RecipeTree
            {
                Name = Name + " x" + quantity
            };

            foreach (IngredientQuantity i in Ingredients.IngredientList)
            {
                ret.AddRecipeNode(new RecipeTree(i.Name + " x" + (i.Quantity * quantity)));
            }

            if(ChildRecipes != null)
            {
                foreach (RecipeQuantity r in ChildRecipes.RecipeList)
                {
                    ret.AddRecipeNode(r.Recipe.GetRecipeNodes(r.Quantity * quantity));
                }
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
