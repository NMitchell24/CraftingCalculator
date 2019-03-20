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
        protected RecipeMap ChildRecipes = new RecipeMap();
        public new string Tooltip
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

                foreach (RecipeQuantity recipe in ChildRecipes.RecipeList)
                {
                    sb.AppendLine(recipe.Name + " x" + recipe.Quantity);
                }

                return sb.ToString();
            }
            protected set { Tooltip = value; }
        }

        override 
        public IngredientMap GetIngredients()
        {
            IngredientMap NewIngredients = Ingredients;

            foreach (RecipeQuantity recipe in ChildRecipes.RecipeList)
            {
                IngredientMap RecipeIngredients = recipe.Ingredients;

                NewIngredients = IngredientUtil.CombineIngredients(RecipeIngredients, NewIngredients, recipe.Quantity);
            }

            return NewIngredients;
        }
    }
}
