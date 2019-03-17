using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CuboidRoom : Recipe
    {
        public CuboidRoom()
        {
            Name = "Cuboid Room";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 200);
        }
    }
}
