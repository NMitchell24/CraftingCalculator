using System.Collections.Generic;
using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes;

namespace CraftingCalculator.Service
{
    public static class RecipeService
    {
        /// <summary>
        /// Loads all Recipes for a provided RecipeFilter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Recipe> GetRecipesByFilter(RecipeFilter filter)
        {
            List<Recipe> ret = new List<Recipe>();

            foreach (RecipeData data in RecipeDAO.GetRecipeDataByFilter(filter))
            {
                ret.Add(GetRecipeForData(data));
            }

            return ret;
        }

        /// <summary>
        /// Get the full Recipe Wrapper object for a provided RecipeData id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Recipe GetRecipeForId(int id)
        {
            Recipe ret = null;
            RecipeData data = RecipeDAO.GetRecipeById(id);

            if(data != null)
            {
                ret = GetRecipeForData(data);
            }

            return ret;
        }

        /// <summary>
        /// Builds a Recipe wrapper object from a provided RecipeData object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Recipe GetRecipeForData(RecipeData data)
        {
            Recipe ret = new Recipe
            {
                Name = data.Name,
                Description = data.Description,
                Type = data.Filter.Name,
                Id = data.Id
            };

            IngredientMap ingMap = new IngredientMap();
            foreach (IngredientQuantityData iData in data.Ingredients)
            {
                IngredientData ing = IngredientDAO.GetIngredientById(iData.Ingredient.Id);
                ingMap.Add(ing.Name, ing.Description, iData.Quantity);
            }

            ret.Ingredients = ingMap;

            List<RecipeQuantityData> children = RecipeDAO.GetRecipeQuantityByParentId(data.Id);

            if (children != null && children.Count > 0)
            {
                RecipeMap recMap = new RecipeMap();
                foreach (RecipeQuantityData recQ in children)
                {
                    RecipeData childData = RecipeDAO.GetRecipeById(recQ.ChildRecipe.Id);
                    Recipe child = GetRecipeForData(childData);
                    recMap.Add(child, recQ.Quantity);
                }
                ret.ChildRecipes = recMap;
            }

            return ret;
        }
    }
}
