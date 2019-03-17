using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class CurvedDesk : Recipe
    {
        public CurvedDesk()
        {
            Name = "Curved Desk";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
