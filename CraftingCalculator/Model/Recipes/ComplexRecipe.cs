﻿using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Utilities;
using System.Text;
using System;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// Abstract Implementation for more complex recipes that include other recipes as ingredients.
    /// When Adding new Recipes they should also be added into the appropriate Get function in RecipeUtil
    /// </summary>
    public abstract class ComplexRecipe : Recipe
    {
        protected IDictionary<Recipe, int> ChildRecipes = new Dictionary<Recipe, int>();
        public new string Tooltip
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Name + " Ingredients:");
                sb.Append(Environment.NewLine);
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
