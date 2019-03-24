using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalRoof : Recipe
    {
        public MetalRoof()
        {
            Name = "Metal Roof";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 30);
        }
    }
}
