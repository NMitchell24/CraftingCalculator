using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class MiningBeamRepair : Recipe
    {
        public MiningBeamRepair()
        {
            Name = "Mining Beam (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.FERRITE_DUST, 30);
        }
    }
}
