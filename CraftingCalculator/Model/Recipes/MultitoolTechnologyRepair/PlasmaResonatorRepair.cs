using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class PlasmaResonatorRepair : Recipe
    {
        public PlasmaResonatorRepair()
        {
            Name = "Plasma Resonator (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.PURE_FERRITE, 25);
        }
    }
}
