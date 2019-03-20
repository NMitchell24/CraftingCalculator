using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// A class that represents a recipe and quantity
    /// </summary>
    class RecipeQuantity
    {
        private Recipe _recipe;
        public int Quantity { get; set; }
        public string CoreComponents { get => _recipe.GetCoreComponents(Quantity); private set { } }
        public string Name { get => _recipe.Name; private set { } }
        public string Type { get => _recipe.Type; private set { } }
        public string Tooltip { get => _recipe.Tooltip; private set { } }
        public IDictionary<IngredientType, int> Ingredients { get => _recipe.GetIngredients(); private set { } }

        public RecipeQuantity(Recipe recipe, int quantity)
        {
            _recipe = recipe;
            Quantity = quantity;
        }
    }
}
