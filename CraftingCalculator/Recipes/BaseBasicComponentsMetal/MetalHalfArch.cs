using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalHalfArch : Recipe
    {
        public MetalHalfArch()
        {
            Name = "Metal Half Arch";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
