using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenWall : Recipe
    {
        public WoodenWall()
        {
            Name = "Wooden Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 40);
        }
    }
}
