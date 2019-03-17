using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class SmallMetalPanel : Recipe
    {
        public SmallMetalPanel()
        {
            Name = "Small Metal Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
