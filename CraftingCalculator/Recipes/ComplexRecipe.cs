using System.Collections.Generic;
using CraftingCalculator.Ingredients;
using CraftingCalculator.Utilities;

namespace CraftingCalculator.Recipes
{
    /// <summary>
    /// Abstract Implementation for more complex recipes that include other recipes as ingredients.
    /// When Adding new Recipes they should also be added into the appropriate Get function in RecipeUtil
    /// </summary>
    public abstract class ComplexRecipe : Recipe
    {
        protected IDictionary<Recipe, int> ChildRecipes = new Dictionary<Recipe, int>();

        override 
        public IDictionary<IngredientType, int> GetIngredients()
        {
            IDictionary<IngredientType, int> NewIngredients = Ingredients;

            foreach (KeyValuePair<Recipe, int> Recipe in ChildRecipes)
            {
                IDictionary<IngredientType, int> RecipeIngredients = Recipe.Key.GetIngredients();

                NewIngredients = IngredientUtil.CombineIngredients(RecipeIngredients, NewIngredients, Recipe.Value);
            }

            return NewIngredients;
        }
    }
}
