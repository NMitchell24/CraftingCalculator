using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
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
