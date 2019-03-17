using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseEquipment
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
