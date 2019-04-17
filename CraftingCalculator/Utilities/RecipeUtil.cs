using System.Collections.Generic;
using System.Linq;
using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.Model.Recipes;
using CraftingCalculator.Model.Ingredients;


namespace CraftingCalculator.Utilities
{
    /// <summary>
    /// For looking up lists of recipes.  
    /// Separate Functions are provided to get lists for UI Filters.
    /// New Recipes should be added into the appropriate Get function 
    /// so that they are displayed in the proper filter.
    /// If a new Get is added for a new filter, then it should also be added 
    /// into the GetAll function so that the new recipes are included in the 'All' filter.
    /// </summary>
    public static class RecipeUtil
    {
        
        /// <summary>
        /// Get a list of RecipeFilters
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilter> GetRecipeFilters()
        {
            List<RecipeFilter> ret = new List<RecipeFilter>();

            foreach(RecipeFilterData data in CraftingCalculatorDAO.GetAllRecipeFiltersData())
            {
                RecipeFilter filter = new RecipeFilter
                {
                    Name = data.Name,
                    Id = data.Id
                };
                ret.Add(filter);
            }

            return ret;
        }

        /// <summary>
        /// Loads all Recipes for a provided RecipeFilter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Recipe> GetRecipesByFilter(RecipeFilter filter)
        {
            List<Recipe> ret = new List<Recipe>();

            foreach (RecipeData data in CraftingCalculatorDAO.GetRecipeDataByFilter(filter))
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
        private static Recipe GetRecipeForData(RecipeData data)
        {
            Recipe ret = new Recipe
            {
                Name = data.Name,
                Type = data.Filter.Name,
            };

            IngredientMap ingMap = new IngredientMap();
            foreach(IngredientQuantityData iData in data.Ingredients)
            {
                IngredientData ing = CraftingCalculatorDAO.GetIngredientById(iData.Ingredient.Id);
                ingMap.Add(ing.Name, iData.Quantity);
            }

            ret.Ingredients = ingMap;

            List<RecipeQuantityData> children = CraftingCalculatorDAO.GetRecipeQuantityByParentId(data.Id);

            if(children != null && children.Count > 0)
            {
                RecipeMap recMap = new RecipeMap();
                foreach(RecipeQuantityData recQ in children)
                {
                    RecipeData childData = CraftingCalculatorDAO.GetRecipeById(recQ.ChildRecipe.Id);
                    Recipe child = GetRecipeForData(childData);
                    recMap.Add(child, recQ.Quantity);
                }
                ret.ChildRecipes = recMap;
            }

            return ret;
        }

    }
}
