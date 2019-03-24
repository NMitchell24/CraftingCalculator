using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalWall : Recipe
    {
        public MetalWall()
        {
            Name = "Metal Wall";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
