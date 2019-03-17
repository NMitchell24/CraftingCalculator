using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalDoorway : Recipe
    {
        public MetalDoorway()
        {
            Name = "Metal Doorway";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
