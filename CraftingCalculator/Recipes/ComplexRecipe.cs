using System.Collections.Generic;
using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    abstract class ComplexRecipe : Recipe
    {
        protected IDictionary<Recipe, int> ChildRecipes = new Dictionary<Recipe, int>();

        override 
        public IDictionary<IngredientType, int> GetIngredients()
        {
            IDictionary<IngredientType, int> NewIngredients = Ingredients;

            foreach (KeyValuePair<Recipe, int> Recipe in ChildRecipes)
            {
                IDictionary<IngredientType, int> RecipeIngredients = Recipe.Key.GetIngredients();
                foreach ( KeyValuePair<IngredientType, int> Ingredient in RecipeIngredients){
                    if (NewIngredients.ContainsKey(Ingredient.Key))
                    {
                        NewIngredients[Ingredient.Key] = NewIngredients[Ingredient.Key]
                            + (Ingredient.Value * Recipe.Value);
                    }
                    else
                    {
                        NewIngredients.Add(Ingredient.Key, (Ingredient.Value * Recipe.Value));
                    }
                }
            }

            return NewIngredients;
        }
    }
}
