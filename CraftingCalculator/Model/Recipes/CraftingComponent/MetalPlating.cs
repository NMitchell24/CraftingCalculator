using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class MetalPlating : Recipe
    {
        public MetalPlating()
        {
            Name = "Metal Plating";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
