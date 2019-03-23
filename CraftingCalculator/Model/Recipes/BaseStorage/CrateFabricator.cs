using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.BaseStorage
{
    class CrateFabricator : ComplexRecipe
    {
        public CrateFabricator()
        {
            Name = "Crate Fabricator";
            Type = RecipeFilterLabels.BaseStorage;
            Ingredients.Add(IngredientType.IONISED_COBALT, 10);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
