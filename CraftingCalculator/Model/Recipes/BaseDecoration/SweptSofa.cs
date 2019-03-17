using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class SweptSofa : Recipe
    {
        public SweptSofa()
        {
            Name = "Swept Sofa";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
