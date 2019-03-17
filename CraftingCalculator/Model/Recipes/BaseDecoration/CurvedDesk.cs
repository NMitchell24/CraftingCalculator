using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
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
