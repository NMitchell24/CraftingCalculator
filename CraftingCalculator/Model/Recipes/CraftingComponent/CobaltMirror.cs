using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class CobaltMirror : Recipe
    {
        public CobaltMirror()
        {
            Name = "Cobalt Mirror";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
        }
    }
}
