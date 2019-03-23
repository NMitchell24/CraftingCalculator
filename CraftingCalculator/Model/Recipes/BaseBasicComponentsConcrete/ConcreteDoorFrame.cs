using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteDoorFrame : Recipe
    {
        public ConcreteDoorFrame()
        {
            Name = "Concrete Door Frame";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
