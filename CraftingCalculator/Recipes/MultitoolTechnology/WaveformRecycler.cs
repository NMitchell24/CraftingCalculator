using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class WaveformRecycler : ComplexRecipe
    {
        public WaveformRecycler()
        {
            Name = "Waveform Recycler";
            Type = "Scanner Module";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 100);
            ChildRecipes.Add(new IonBattery(), 1);
        }
    }
}
