using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseAquaticConstruction
{
    class WatertightDoor : Recipe
    {
        public WatertightDoor()
        {
            Name = "Watertight Door";
            Type = RecipeFilterLabels.BaseAquaticConstruction;
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 1);
        }
    }
}
