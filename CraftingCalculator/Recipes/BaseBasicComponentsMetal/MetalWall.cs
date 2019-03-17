using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalWall : Recipe
    {
        public MetalWall()
        {
            Name = "Metal Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
