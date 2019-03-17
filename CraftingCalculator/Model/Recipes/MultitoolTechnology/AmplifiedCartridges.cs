using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
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
