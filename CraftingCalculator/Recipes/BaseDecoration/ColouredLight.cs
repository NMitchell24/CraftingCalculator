using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class ColouredLight : Recipe
    {
        public ColouredLight()
        {
            Name = "Coloured Light (All)";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.SODIUM, 5);
        }
    }
}
