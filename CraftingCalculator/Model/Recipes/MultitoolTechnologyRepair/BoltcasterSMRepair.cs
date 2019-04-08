using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class BoltcasterSMRepair : Recipe
    {
        public BoltcasterSMRepair()
        {
            Name = "Boltcaster SM (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.PURE_FERRITE, 25);
        }
    }
}
