using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalDoorFrame : Recipe
    {
        public MetalDoorFrame()
        {
            Name = "Metal Door Frame";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
