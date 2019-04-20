using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.Model.Recipes;
using System.Collections.Generic;

namespace CraftingCalculator.Service
{
    public static class RecipeFilterService
    {
        /// <summary>
        /// Get a list of RecipeFilters
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilter> GetRecipeFilters()
        {
            List<RecipeFilter> ret = new List<RecipeFilter>();

            foreach (RecipeFilterData data in RecipeFilterDAO.GetAllRecipeFiltersData())
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
    }
}
