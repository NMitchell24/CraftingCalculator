using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CuboidInnerDoor : Recipe
    {
        public CuboidInnerDoor()
        {
            Name = "Cuboid Inner Door";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
        }
    }
}
