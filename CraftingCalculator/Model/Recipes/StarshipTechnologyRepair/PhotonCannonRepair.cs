using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class PhotonCannonRepair : Recipe
    {
        public PhotonCannonRepair()
        {
            Name = "Photon Cannon (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 30);
        }
    }
}
