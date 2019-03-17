using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
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
