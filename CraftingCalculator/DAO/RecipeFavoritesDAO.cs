using CraftingCalculator.Model.Data;
using LiteDB;
using System.Collections.Generic;
using System.Linq;


namespace CraftingCalculator.DAO
{
    public class RecipeFavoritesDAO : AbstractDAO
    {
        /// <summary>
        /// Returns all the Recipe Favorites
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFavoritesData> GetAllRecipeFavorites()
        {
            List<RecipeFavoritesData> ret = new List<RecipeFavoritesData>();
            var col = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);

            ret.AddRange(col.Find(Query.All(Query.Ascending))
                .OrderBy(x => x.Name));

            return ret;

        }

        public static List<FavoriteRecipeQuantitiesData> GetRecipeQuantitesForFavoriteId(int id)
        {
            List<FavoriteRecipeQuantitiesData> ret = new List<FavoriteRecipeQuantitiesData>();
            var col = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);

            ret.AddRange(col.Include(x => x.Favorite)
                .Include(x => x.Recipe)
                .Find(x => x.Favorite.Id == id));

            return ret;

        }

        /// <summary>
        /// Saves or updates a RecipeFavoritesData object to the database.
        /// </summary>
        /// <param name="fav"></param>
        public static void SaveRecipeFavorite(RecipeFavoritesData fav)
        {
            if (fav != null)
            {
                var col = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);

                col.Upsert(fav);
            }
        }

        /// <summary>
        /// Looks up a favorite by name.
        /// Using Name instead of ID to make sure that Names stay unique.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static RecipeFavoritesData GetFavoriteByName(string? name)
        {
            if (name != null)
            {
                var col = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);
                return col.FindOne(x => x.Name == name);
            } 
            else
            { 
                return new RecipeFavoritesData(); 
            }
        }

        /// <summary>
        /// Saves or updates a FavoriteRecipeQuantitiesData object to the database.
        /// </summary>
        /// <param name="favs"></param>
        public static void SaveAllFavoritesRecipeQuantites(List<FavoriteRecipeQuantitiesData> favs)
        {
            var col = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);

            foreach (FavoriteRecipeQuantitiesData data in favs)
            {
                col.Upsert(data);
            }
        }

        /// <summary>
        /// Deletes all data for a provided Favorite.  
        /// if favorite passed in is null we do nothing.
        /// </summary>
        /// <param name="data"></param>
        public static void DeleteFavoritesData(RecipeFavoritesData? data)
        {
            if (data != null)
            {
                foreach (FavoriteRecipeQuantitiesData recData in GetRecipeQuantitesForFavoriteId(data.Id))
                {
                    DeleteRecordById<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities, recData.Id);
                }

                DeleteRecordById<RecipeFavoritesData>(CollectionLabels.RecipeFavorites, data.Id);
            }
        }

        /// <summary>
        /// Returns a list of Favorite Quantities that use the provided Recipe ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<FavoriteRecipeQuantitiesData> GetFavoriteQuantitiesByRecipeId(int id)
        {
            List<FavoriteRecipeQuantitiesData> ret = new List<FavoriteRecipeQuantitiesData>();
            var col = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);
            ret.AddRange(col.Include(x => x.Favorite)
                .Include(x => x.Recipe)
                .Find(x => x.Recipe.Id == id));

            return ret;
        }
    }
}
