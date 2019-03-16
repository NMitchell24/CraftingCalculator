using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class CobaltMirror : Recipe
    {
        public CobaltMirror()
        {
            Name = "Cobalt Mirror";
            Type = "Component";
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
        }
    }
}
