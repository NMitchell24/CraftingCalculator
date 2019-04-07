using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class ScannerRepair : Recipe
    {
        public ScannerRepair()
        {
            Name = "Scanner (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.FERRITE_DUST, 75);
        }
    }
}
