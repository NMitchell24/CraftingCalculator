using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class ThinWoodenWall : Recipe
    {
        public ThinWoodenWall()
        {
            Name = "Thin Wooden Wall";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
