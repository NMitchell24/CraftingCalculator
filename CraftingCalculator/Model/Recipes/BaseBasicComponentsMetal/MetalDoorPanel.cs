using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalDoorPanel : Recipe
    {
        public MetalDoorPanel()
        {
            Name = "Metal Door Panel";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
