using System.Collections.Generic;
using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.ViewModel.Ingredients;
using CraftingCalculator.ViewModel.Recipes;

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
            return GetRecipeListForDataList(RecipeDAO.GetRecipeDataByFilter(filter));
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
        /// Convert a list of Data records to DTO records.
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        private static List<Recipe> GetRecipeListForDataList(List<RecipeData> dataList)
        {
            List<Recipe> ret = new List<Recipe>();

            foreach (RecipeData data in dataList)
            {
                ret.Add(GetRecipeForData(data));
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
                FilterType = data.Filter != null ? data.Filter.Name : null,
                Id = data.Id
            };

            IngredientMap ingMap = new IngredientMap();
            foreach (IngredientQuantityData iData in data.Ingredients)
            {
                IngredientData ing = AbstractDAO.GetRecordById<IngredientData>(CollectionLabels.Ingredients, iData.Ingredient.Id);
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

        public static List<Recipe> GetAllRecipes()
        {
            return GetRecipeListForDataList(RecipeDAO.GetAllRecipeData());
        }
    }
}
