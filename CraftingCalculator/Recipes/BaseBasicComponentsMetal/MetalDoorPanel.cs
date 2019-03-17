using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalDoorPanel : Recipe
    {
        public MetalDoorPanel()
        {
            Name = "Metal Door Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
