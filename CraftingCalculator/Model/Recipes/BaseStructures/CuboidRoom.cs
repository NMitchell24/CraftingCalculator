using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CuboidRoom : Recipe
    {
        public CuboidRoom()
        {
            Name = "Cuboid Room";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 200);
        }
    }
}
