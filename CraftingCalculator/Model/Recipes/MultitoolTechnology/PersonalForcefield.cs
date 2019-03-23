using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class PersonalForcefield : ComplexRecipe
    {
        public PersonalForcefield()
        {
            Name = "Personal Forcefield";
            Type = RecipeFilterLabels.MultitoolTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new IonBattery(), 2);
        }
    }
}
