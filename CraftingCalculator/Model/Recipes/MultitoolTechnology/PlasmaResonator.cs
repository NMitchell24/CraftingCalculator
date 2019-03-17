using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class PlasmaResonator : Recipe
    {
        public PlasmaResonator()
        {
            Name = "Plasma Resonator";
            Type = "Mining Beam Module";
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
        }
    }
}
