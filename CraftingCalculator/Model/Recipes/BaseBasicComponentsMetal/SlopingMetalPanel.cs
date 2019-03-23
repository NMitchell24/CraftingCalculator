using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class SlopingMetalPanel : Recipe
    {
        public SlopingMetalPanel()
        {
            Name = "Sloping Metal Panel";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
