using CraftingCalculator.Model.Data;
using CraftingCalculator.Model.Recipes;
using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.DAO
{
    public static class RecipeDAO
    {      
        private static DataManager _data = DataManager.Instance;

        /// <summary>
        /// Returns all Recipes for a given filter.   
        /// If the Filter name is 'All' then we will return all recipes.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<RecipeData> GetRecipeDataByFilter(RecipeFilter filter)
        {
            List<RecipeData> ret = new List<RecipeData>();
            var col = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);

            if (filter.Name == RecipeFilter.ALL)
            {
                ret.AddRange(col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .Find(Query.All(Query.Ascending))
                .OrderBy(x => x.Name));
            }
            else
            {
                ret.AddRange(col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .Find(x => x.Filter.Id == filter.Id)
                .OrderBy(x => x.Name));
            }
            
            return ret;
        }
        
        /// <summary>
        /// Gets all Recipe Quantity objects for a given Parent Recipe id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<RecipeQuantityData> GetRecipeQuantityByParentId(int id)
        {
            List<RecipeQuantityData> ret = new List<RecipeQuantityData>();
            var col = _data.GetCollectionByType<RecipeQuantityData>(CollectionLabels.RecipeQuantities);

            ret.AddRange(col.Include(x => x.ParentRecipe)
                .Include(x => x.ChildRecipe)
                .Find(x => x.ParentRecipe.Id == id));

            return ret;
        }

        /// <summary>
        /// Gets a Recipe by the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static RecipeData GetRecipeById(int id)
        {
            var col = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);

            return col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .FindById(id);
        }   
        
    }
}
