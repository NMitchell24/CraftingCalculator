using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class Decal : Recipe
    {
        public Decal()
        {
            Name = "Decal (All)";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
