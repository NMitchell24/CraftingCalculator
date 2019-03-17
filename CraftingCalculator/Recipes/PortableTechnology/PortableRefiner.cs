using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class PortableRefiner : ComplexRecipe
    {
        public PortableRefiner()
        {
            Name = "Portable Refiner";
            Type = "Portable Base Technology";
            Ingredients.Add(IngredientType.OXYGEN, 30);
            ChildRecipes.Add(new MetalPlating(), 1);
        }
    }
}
