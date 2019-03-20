using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Utilities;
using System.Text;
using System;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// Abstract Implementation for more complex recipes that include other recipes as ingredients.
    /// When Adding new Recipes they will show up automagically as long as they are added to an existing Namespace.
    /// If creating a new Namespace then you must add a RecipeType as well, 
    /// and then create a filter for that type in RecipeUtil.
    /// </summary>
    public abstract class ComplexRecipe : Recipe
    {
        protected IDictionary<Recipe, int> ChildRecipes = new Dictionary<Recipe, int>();
        public new string Tooltip
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

                foreach(KeyValuePair<Recipe, int> recipe in ChildRecipes)
                {
                    sb.AppendLine(recipe.Key.Name + " x" + recipe.Value);
                }

                return sb.ToString();
            }
            protected set { Tooltip = value; }
        }

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
