using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalDoorway : Recipe
    {
        public MetalDoorway()
        {
            Name = "Metal Doorway";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
