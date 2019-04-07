using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class HyperdriveRepair : ComplexRecipe
    {
        public HyperdriveRepair()
        {
            Name = "Hyperdrive (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 63);
            ChildRecipes.Add(new Microprocessor(), 3);
        }
    }
}
