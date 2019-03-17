using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class ThinMetalWall : Recipe
    {
        public ThinMetalWall()
        {
            Name = "Thin Metal Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
