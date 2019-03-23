using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class WarpCell : ComplexRecipe
    {
        public WarpCell()
        {
            Name = "Warp Cell";
            Type = RecipeFilterLabels.Consumables;
            ChildRecipes.Add(new AntimatterHousing(), 1);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
