using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.CraftingComponent
{
    class AntimatterHousing : Recipe
    {
        public AntimatterHousing()
        {
            Name = "Antimatter Housing";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.OXYGEN, 30);
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
