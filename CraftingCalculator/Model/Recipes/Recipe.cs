using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;
using System.Text;
using System;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// Abstract Recipe Implementation for recipes that contain only basic Elements as ingredients.
    /// When Adding new Recipes they should also be added into the appropriate Get function in RecipeUtil
    /// </summary>
    public abstract class Recipe 
    {

        protected IDictionary<IngredientType, int> Ingredients = new Dictionary<IngredientType, int>();
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

                foreach (KeyValuePair<IngredientType, int> ingredient in Ingredients)
                {
                    sb.AppendLine(ingredient.Key.GetDisplayName() + " x" + ingredient.Value);
                }

                return sb.ToString();
            }
            protected set { Tooltip = value;}
        }
       

        public virtual IDictionary<IngredientType, int> GetIngredients()
        {
            return Ingredients;
        }


    }
}
