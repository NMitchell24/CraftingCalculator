using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CurvedCuboidWall : Recipe
    {
        public CurvedCuboidWall()
        {
            Name = "Curved Cuboid Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
