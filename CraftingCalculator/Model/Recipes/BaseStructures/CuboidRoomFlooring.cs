using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CuboidRoomFlooring : Recipe
    {
        public CuboidRoomFlooring()
        {
            Name = "Cuboid Room Flooring";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
