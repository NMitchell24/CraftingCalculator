using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.ViewModel.Recipes;
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
                RecipeFilter filter = GetFilterForData(data);
                ret.Add(filter);
            }

            return ret;
        }

        /// <summary>
        /// Return a RecipeFilter object by ID.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static RecipeFilter GetRecipeFilterById(int Id)
        {
            return GetFilterForData(AbstractDAO.GetRecordById<RecipeFilterData>(CollectionLabels.RecipeFilters, Id));
        }

        /// <summary>
        /// Saves or adds the Recipe Filter.  If the ID value of the provided filter is 0 a new one will be added.  otherwise this will update the existing filter.
        /// </summary>
        /// <param name="filter"></param>
        public static void SaveRecipeFilter(RecipeFilter filter)
        {
            RecipeFilterData data;
            if (filter.Id > 0)
            {
                data = AbstractDAO.GetRecordById<RecipeFilterData>(CollectionLabels.RecipeFilters, filter.Id);
            }
            else
            {
                data = new RecipeFilterData();
            }

            CopyToData(filter, data);

            AbstractDAO.AddOrUpdateRecord<RecipeFilterData>(CollectionLabels.RecipeFilters, data, filter.Id <= 0);
        }

        /// <summary>
        /// Deletes an existing Recipe Filter
        /// </summary>
        /// <param name="filter"></param>
        public static void DeleteRecipeFilter(RecipeFilter filter)
        {
            //First remove filter reference from any Recipe objects.
            List<RecipeData> recipes = RecipeDAO.GetRecipeDataByFilter(filter);
            foreach (RecipeData recipe in recipes)
            {
                recipe.Filter = null;
                AbstractDAO.AddOrUpdateRecord<RecipeData>(CollectionLabels.Recipes, recipe, false);
            }

            //Delete Filter
            AbstractDAO.DeleteRecordById<RecipeFilterData>(CollectionLabels.RecipeFilters, filter.Id);
        }

        /// <summary>
        /// Convert the Data object into the RecipeFilter object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static RecipeFilter GetFilterForData(RecipeFilterData data)
        {
            return new RecipeFilter
            {
                Description = data.Description,
                Name = data.Name,
                Id = data.Id
            };
        }

        private static void CopyToData(RecipeFilter filter, RecipeFilterData data)
        {
            data.Name = filter.Name;
            data.Description = filter.Description;
        }
    }
}
