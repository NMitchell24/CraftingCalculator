using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Server : Recipe
    {
        public Server()
        {
            Name = "Server";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
