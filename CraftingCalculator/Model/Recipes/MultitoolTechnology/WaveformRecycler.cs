using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
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
