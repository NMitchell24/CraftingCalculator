using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class Server : Recipe
    {
        public Server()
        {
            Name = "Server";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
