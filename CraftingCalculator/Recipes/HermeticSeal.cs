using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class HermeticSeal : Recipe
    {
        public HermeticSeal()
        {
            Name = "Hermetic Seal";
            Type = "Component";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 30);
        }
    }
}
