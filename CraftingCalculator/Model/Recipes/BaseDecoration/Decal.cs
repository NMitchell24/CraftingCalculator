using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
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
