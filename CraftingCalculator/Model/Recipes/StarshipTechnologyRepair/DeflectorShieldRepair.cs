using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class DeflectorShieldRepair : Recipe
    {
        public DeflectorShieldRepair()
        {
            Name = "Deflector Shield (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 13);
        }
    }
}
