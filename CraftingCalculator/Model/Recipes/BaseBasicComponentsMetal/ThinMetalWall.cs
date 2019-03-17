using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
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
