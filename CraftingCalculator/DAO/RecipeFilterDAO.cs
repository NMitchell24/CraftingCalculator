using System.Collections.Generic;
using System.Linq;
using CraftingCalculator.Model.Data;
using LiteDB;
using CraftingCalculator.Model.Recipes;

namespace CraftingCalculator.DAO
{
    public class RecipeFilterDAO : AbstractDAO
    {
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
