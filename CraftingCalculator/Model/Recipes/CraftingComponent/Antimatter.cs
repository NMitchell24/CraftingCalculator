using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class Antimatter : Recipe
    {
        public Antimatter()
        {
            Name = "Antimatter";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 20);
        }
    }
}
