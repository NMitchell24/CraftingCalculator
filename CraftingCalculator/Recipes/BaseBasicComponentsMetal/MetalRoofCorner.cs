using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalRoofCorner : Recipe
    {
        public MetalRoofCorner()
        {
            Name = "Metal Roof Corner";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
        }
    }
}
