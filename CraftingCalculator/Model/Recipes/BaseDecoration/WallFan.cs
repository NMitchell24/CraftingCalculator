using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class WallFan : Recipe
    {
        public WallFan()
        {
            Name = "Wall Fan";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
