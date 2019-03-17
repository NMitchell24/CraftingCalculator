using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class SidePanel : Recipe
    {
        public SidePanel()
        {
            Name = "Side Panel";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
            Ingredients.Add(IngredientType.SODIUM, 10);
        }
    }
}
