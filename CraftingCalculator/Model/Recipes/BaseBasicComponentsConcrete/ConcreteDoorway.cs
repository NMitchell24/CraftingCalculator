using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteDoorway : Recipe
    {
        public ConcreteDoorway()
        {
            Name = "Concrete Doorway";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
