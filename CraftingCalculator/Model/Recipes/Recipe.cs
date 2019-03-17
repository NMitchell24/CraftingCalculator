using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;

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

        public virtual IDictionary<IngredientType, int> GetIngredients()
        {
            return Ingredients;
        }
    }
}
