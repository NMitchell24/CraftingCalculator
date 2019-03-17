using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalArch : Recipe
    {
        public MetalArch()
        {
            Name = "Metal Arch";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
