using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.ViewModel.Recipes;
using System.Collections.Generic;

namespace CraftingCalculator.Service
{
    public static class RecipeFavoriteService
    {
        /// <summary>
        /// Returns all the Recipe Favorite objects.
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFavorite> GetAllRecipeFavorites()
        {
            List<RecipeFavorite> ret = new List<RecipeFavorite>();

            foreach (RecipeFavoritesData data in RecipeFavoritesDAO.GetAllRecipeFavorites())
            {
                RecipeFavorite fav = new RecipeFavorite
                {
                    Name = data.Name,
                    Id = data.Id
                };
                ret.Add(fav);
            }
            return ret;
        }

        /// <summary>
        /// Saves or updates a Recipe Favorite
        /// </summary>
        /// <param name="fav"></param>
        public static void SaveRecipeFavorite(RecipeFavorite fav, List<RecipeQuantity> quantities)
        {
            //First delete existing data (if it exists)
            DeleteFavoriteData(fav);

            RecipeFavoritesData data = new RecipeFavoritesData()
            {
                Name = fav.Name
            };

            if (fav.Id > 0)
            {
                data.Id = fav.Id;
            }
            //Save or update the Favorite object first.
            RecipeFavoritesDAO.SaveRecipeFavorite(data);

            //Look the favorite back up from the DB to make sure we get the correct ID to reference.
            data = RecipeFavoritesDAO.GetFavoriteByName(fav.Name);

            //Build a list for the recipe quantities.
            List<FavoriteRecipeQuantitiesData> favsData = new List<FavoriteRecipeQuantitiesData>();

            foreach (RecipeQuantity q in quantities)
            {
                RecipeData recData = RecipeDAO.GetRecipeById(q.Recipe.Id);
                FavoriteRecipeQuantitiesData recFavData = new FavoriteRecipeQuantitiesData()
                {
                    Favorite = data,
                    Recipe = recData,
                    Quantity = q.Quantity
                };

                favsData.Add(recFavData);
            }

            //Save the recipe quantities for this favorite.
            RecipeFavoritesDAO.SaveAllFavoritesRecipeQuantites(favsData);
        }

        /// <summary>
        /// Deletes the data for a provided favorite.
        /// </summary>
        /// <param name="fav"></param>
        public static void DeleteFavoriteData(RecipeFavorite fav)
        {
            RecipeFavoritesData data = RecipeFavoritesDAO.GetFavoriteByName(fav.Name);

            RecipeFavoritesDAO.DeleteFavoritesData(data);
        }

        /// <summary>
        /// Check to see if a Favorite already exists in the DB
        /// </summary>
        /// <param name="fav"></param>
        /// <returns></returns>
        public static bool DoesFavoriteExist(string fav)
        {
            return (RecipeFavoritesDAO.GetFavoriteByName(fav) != null);
        }

        /// <summary>
        /// Return the RecipeQuantity list for a provided Favorite
        /// </summary>
        /// <param name="fav"></param>
        /// <returns></returns>
        public static List<RecipeQuantity> GetRecipeQuantitiesForFavorite(RecipeFavorite fav)
        {
            List<RecipeQuantity> ret = new List<RecipeQuantity>();

            List<FavoriteRecipeQuantitiesData> data = RecipeFavoritesDAO.GetRecipeQuantitesForFavoriteId(fav.Id);

            foreach (FavoriteRecipeQuantitiesData favRecData in data)
            {
                Recipe rec = RecipeService.GetRecipeForId(favRecData.Recipe.Id);
                if(rec != null)
                {
                    ret.Add(new RecipeQuantity(rec, favRecData.Quantity, 0));
                }
            }

            return ret;
        }
    }
}
