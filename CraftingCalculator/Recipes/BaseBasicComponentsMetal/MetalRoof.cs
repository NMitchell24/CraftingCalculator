using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalRoof : Recipe
    {
        public MetalRoof()
        {
            Name = "Metal Roof";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 30);
        }
    }
}
