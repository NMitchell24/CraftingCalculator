using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseAquaticConstruction
{
    class MoonpoolFloor : Recipe
    {
        public MoonpoolFloor()
        {
            //I really want to call this moon poop floor, on 'accident'....
            Name = "Moon Pool Floor";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 350);
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 6);
            Ingredients.Add(IngredientType.LIVING_PEARL, 2);
        }
    }
}
