using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class SmallMetalWall : Recipe
    {
        public SmallMetalWall()
        {
            Name = "Small Metal Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
