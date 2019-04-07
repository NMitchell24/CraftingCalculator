using CraftingCalculator.Model.Recipes.Consumable;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class WaveformRecyclerRepair : ComplexRecipe
    {
        public WaveformRecyclerRepair()
        {
            Name = "Waveform Recycler (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 50);
            ChildRecipes.Add(new IonBattery(), 1);
        }
    }
}
