using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class BoltcasterSM : Recipe
    {
        public BoltcasterSM()
        {
            Name = "Boltcaster SM";
            Type = "Boltcaster Module";
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
        }
    }
}
