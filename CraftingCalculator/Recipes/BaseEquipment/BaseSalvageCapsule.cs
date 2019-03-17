using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseEquipment
{
    class BaseSalvageCapsule : Recipe
    {
        public BaseSalvageCapsule()
        {
            Name = "Base Salvage Capsule";
            Type = "Base Equipment";
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
        }
    }
}
