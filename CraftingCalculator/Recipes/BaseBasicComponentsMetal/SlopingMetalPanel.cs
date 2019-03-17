using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class SlopingMetalPanel : Recipe
    {
        public SlopingMetalPanel()
        {
            Name = "Sloping Metal Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
