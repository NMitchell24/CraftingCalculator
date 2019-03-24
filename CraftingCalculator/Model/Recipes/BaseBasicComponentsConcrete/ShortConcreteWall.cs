using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ShortConcreteWall : Recipe
    {
        public ShortConcreteWall()
        {
            Name = "Short Concrete Wall";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
