using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class HermeticSeal : Recipe
    {
        public HermeticSeal()
        {
            Name = "Hermetic Seal";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 30);
        }
    }
}
