using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.BaseStorage
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
