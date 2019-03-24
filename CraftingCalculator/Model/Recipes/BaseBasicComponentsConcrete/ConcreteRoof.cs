using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteRoof : Recipe
    {
        public ConcreteRoof()
        {
            Name = "Concrete Roof";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
