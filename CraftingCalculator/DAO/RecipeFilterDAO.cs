using System;
using System.Collections.Generic;
using System.Linq;
using CraftingCalculator.Model.Data;
using LiteDB;
using CraftingCalculator.Model.Recipes;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.DAO
{
    public static class RecipeFilterDAO
    {
        private static DataManager _data = DataManager.Instance;
        /// <summary>
        /// Gets all the Recipe Filter data objects
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilterData> GetAllRecipeFiltersData()
        {
            var col = _data.GetCollectionByType<RecipeFilterData>(CollectionLabels.RecipeFilters);
            List<RecipeFilterData> ret = new List<RecipeFilterData>();

            ret.AddRange(col.Find(Query.All(Query.Ascending))
                .OrderByDescending(x => x.Name == RecipeFilter.ALL).ThenBy(x => x.Name));
            return ret;
        }
    }
}
