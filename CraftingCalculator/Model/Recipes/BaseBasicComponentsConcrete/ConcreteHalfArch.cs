using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteHalfArch : Recipe
    {
        public ConcreteHalfArch()
        {
            Name = "Concrete Half Arch";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
