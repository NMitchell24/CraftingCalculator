using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class SmallMetalWall : Recipe
    {
        public SmallMetalWall()
        {
            Name = "Small Metal Wall";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
