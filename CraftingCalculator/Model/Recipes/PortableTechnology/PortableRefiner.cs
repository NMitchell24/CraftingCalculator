using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
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
