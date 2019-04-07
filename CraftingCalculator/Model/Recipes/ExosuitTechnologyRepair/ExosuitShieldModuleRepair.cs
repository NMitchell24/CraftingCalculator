using CraftingCalculator.Model.Recipes.Consumable;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class ExosuitShieldModuleRepair : ComplexRecipe
    {
        public ExosuitShieldModuleRepair()
        {
            Name = "Exosuit Shield Module (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.SODIUM, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new IonBattery(), 1);
        }
    }
}
