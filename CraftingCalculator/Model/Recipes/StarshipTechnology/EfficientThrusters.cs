using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class EfficientThrusters : ComplexRecipe
    {
        public EfficientThrusters()
        {
            Name = "Efficient Thrusters";
            Type = "Starship Thruster Module";
            Ingredients.Add(IngredientType.TRITIUM, 100);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new DihydrogenJelly(), 1);
        }
    }
}
