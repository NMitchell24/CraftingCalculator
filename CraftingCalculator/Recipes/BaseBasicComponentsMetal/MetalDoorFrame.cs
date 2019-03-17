using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalDoorFrame : Recipe
    {
        public MetalDoorFrame()
        {
            Name = "Metal Door Frame";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
