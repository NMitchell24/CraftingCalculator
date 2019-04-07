using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class HazardProtectionRepair : Recipe
    {
        public HazardProtectionRepair()
        {
            Name = "Hazard Protection Unit (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
