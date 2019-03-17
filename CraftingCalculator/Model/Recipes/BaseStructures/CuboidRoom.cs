using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
