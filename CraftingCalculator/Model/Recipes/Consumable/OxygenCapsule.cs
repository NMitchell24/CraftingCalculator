using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class OxygenCapsule : Recipe
    {
        public OxygenCapsule()
        {
            Name = "Oxygen Capsule";
            Type = "Consumable (Exosuit)";
            Ingredients.Add(IngredientType.OXYGEN, 25);
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
        }
    }
}
