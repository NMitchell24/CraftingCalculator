using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class AmplifiedCartridges : ComplexRecipe
    {
        public AmplifiedCartridges()
        {
            Name = "Amplified Cartridges";
            Type = "Pulse Splitter Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new TetraCobalt(), 1);
        }
    }
}
