using CraftingCalculator.Model.Recipes.Consumable;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class PhotonCannonModuleRepair : ComplexRecipe
    {
        public PhotonCannonModuleRepair()
        {
            Name = "Photon Cannon Module (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.CADMIUM, 125);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new IonBattery(), 1);
        }
    }
}
