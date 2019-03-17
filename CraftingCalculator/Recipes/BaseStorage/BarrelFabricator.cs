using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.BaseStorage
{
    class BarrelFabricator : ComplexRecipe
    {
        public BarrelFabricator()
        {
            Name = "Barrel Fabricator";
            Type = "Base Storage";
            Ingredients.Add(IngredientType.IONISED_COBALT, 10);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
