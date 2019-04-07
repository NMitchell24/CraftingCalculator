using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class LifeSupportRepair : Recipe
    {
        public LifeSupportRepair()
        {
            Name = "Life Support (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
