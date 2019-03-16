using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class Antimatter : Recipe
    {
        public Antimatter()
        {
            Name = "Antimatter";
            Type = "Component";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 20);
        }
    }
}
