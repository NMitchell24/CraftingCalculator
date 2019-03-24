using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class AntimatterHousing : Recipe
    {
        public AntimatterHousing()
        {
            Name = "Antimatter Housing";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.OXYGEN, 30);
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
