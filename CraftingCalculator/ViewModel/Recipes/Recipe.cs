using CraftingCalculator.ViewModel.Ingredients;
using CraftingCalculator.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace CraftingCalculator.ViewModel.Recipes
{
    /// <summary>
    /// Represents an individual UI Model for the Recipes
    /// </summary>
    public class Recipe : AbstractPropertyChanged, IBaseDataRecord
    {
        public IngredientMap Ingredients { get; set; }
        public RecipeMap ChildRecipes { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string FilterType { get; set; }
        public double Value { get; set; }

        public string Tooltip
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Name);
                sb.AppendLine(FilterType);
                sb.Append(Environment.NewLine);
                if(Description != null && Description.Length > 0)
                {
                    sb.AppendLine("Description:");
                    sb.AppendLine(Description);
                    sb.Append(Environment.NewLine);
                }
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

        public DataType Type
        {
            get
            {
                return DataType.Recipe;
            }
            //Don't allow this to be changed as it should remain static.
            set { }
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
                Name = Name + " x" + quantity,
                Id = Name
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

        public IBaseDataRecord Clone()
        {
            Recipe clone = new Recipe()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                FilterType = this.FilterType,
                Value = this.Value,
                Ingredients = this.Ingredients,
                ChildRecipes = this.ChildRecipes
            };

            return clone;
        }

        public IBaseDataRecord CopyForSave()
        {
            Recipe ret = (Recipe)Clone();
            ret.Name = ret.Name + " - Copy";
            ret.Id = 0;

            return ret;
        }
    }
}
