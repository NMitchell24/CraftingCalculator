using System.Collections.Generic;
using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    abstract class Recipe
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
