using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class LargeWedge : Recipe
    {
        public LargeWedge()
        {
            Name = "Large Wedge";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
