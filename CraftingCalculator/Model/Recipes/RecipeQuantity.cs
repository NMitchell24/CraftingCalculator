using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// A class that represents a recipe and quantity
    /// </summary>
    public class RecipeQuantity
    {
        public Recipe Recipe { get; set; }
        public int Quantity { get; set; }
        public string CoreComponents { get => Recipe.GetCoreComponents(Quantity); private set { } }
        public string Name { get => Recipe.Name; private set { } }
        public string Type { get => Recipe.Type; private set { } }
        public string Tooltip { get => Recipe.Tooltip; private set { } }
        public IngredientMap Ingredients { get => Recipe.GetIngredients(); private set { } }

        public RecipeQuantity(Recipe recipe, int quantity)
        {
            Recipe = recipe;
            Quantity = quantity;
        }
    }
}
