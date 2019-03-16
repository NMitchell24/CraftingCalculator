using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.CraftingComponent
{
    class MetalPlating : Recipe
    {
        public MetalPlating()
        {
            Name = "Metal Plating";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
