using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class MetalPlating : Recipe
    {
        public MetalPlating()
        {
            Name = "Metal Plating";
            Type = "Component";
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
