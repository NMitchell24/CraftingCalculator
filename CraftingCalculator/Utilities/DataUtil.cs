using CraftingCalculator.Model.LiteDB;
using CraftingCalculator.Model.Recipes;
using LiteDB;
using System.Collections.Generic;
using System.IO;

namespace CraftingCalculator.Utilities
{
    public static class DataUtil
    {
        /// <summary>
        /// This method will make sure the database exists, all the correct Documents (tables) exist, 
        /// and that the appropriate indexes are created
        /// </summary>
        public static void EnsureDatabaseExists()
        {
            // Creates Default Database from No Mans Sky configuration if no Database currently exists.
            // Prevents program from starting with blank DB.
            if (!File.Exists("CraftingCalculator.db"))
            {
                File.WriteAllBytes(@"CraftingCalculator.db", Properties.Resources.CraftingCalculator);
            }
            
            //Return the db.  Creates if it does not exist.
            var db = DataManager.Instance.GetDatabase();
            //Gets ingredients.  creates Collection if it does not exist.
            var ing = db.GetCollection<IngredientData>(CollectionLabels.Ingredients);
            //Create indexes for ingredients if they do not exist.
            ing.EnsureIndex(x => x.Name);
            ing.EnsureIndex(x => x.Id);

            //Get Filters and create indexes.
            var filt = db.GetCollection<RecipeFilterData>(CollectionLabels.RecipeFilters);
            filt.EnsureIndex(x => x.Name);
            filt.EnsureIndex(x => x.Id);

            //Ingredient Quantity objects.
            var ingQ = db.GetCollection<IngredientQuantityData>(CollectionLabels.IngredientQuantities);
            ingQ.EnsureIndex(x => x.Id);
            ingQ.EnsureIndex(x => x.Ingredient.Id);

            //recipe objects.
            var rec = db.GetCollection<RecipeData>(CollectionLabels.Recipes);
            rec.EnsureIndex(x => x.Id);
            rec.EnsureIndex(x => x.Filter.Id);
            rec.EnsureIndex(x => x.Name);

            //Recipe quantities
            var recQ = db.GetCollection<RecipeQuantityData>(CollectionLabels.RecipeQuantities);
            recQ.EnsureIndex(x => x.Id);
            recQ.EnsureIndex(x => x.ChildRecipe.Id);
            recQ.EnsureIndex(x => x.ParentRecipe.Id);

        }

        /// <summary>
        /// Gets all the Recipe Filter data objects
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilterData> GetAllRecipeFiltersData()
        {
            var db = DataManager.Instance.GetDatabase();
            var col = db.GetCollection<RecipeFilterData>(CollectionLabels.RecipeFilters);
            List<RecipeFilterData> ret = new List<RecipeFilterData>();

            var filters = col.Find(Query.All(Query.Ascending));
            ret.AddRange(filters);
            return ret;
        }

        /// <summary>
        /// Returns all Recipes for a given filter.   
        /// If the Filter name is 'All' then we will return all recipes.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<RecipeData> GetRecipesByFilterWithIngredients(RecipeFilter filter)
        {
            List<RecipeData> ret = new List<RecipeData>();
            var db = DataManager.Instance.GetDatabase();
            var col = db.GetCollection<RecipeData>(CollectionLabels.Recipes);

            if (filter.Name == "All")
            {
                ret.AddRange(col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .Find(Query.All(Query.Ascending)));
            }
            else
            {
                ret.AddRange(col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .Find(x => x.Filter.Id == filter.Id));
            }
            
            return ret;
        }

        /// <summary>
        /// Gets an Ingredient by its Identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IngredientData GetIngredientById(int id)
        {
            var db = DataManager.Instance.GetDatabase();
            var col = db.GetCollection<IngredientData>(CollectionLabels.Ingredients);

            return col.FindById(id);
        }
        
        /// <summary>
        /// Gets all Recipe Quantity objects for a given Parent Recipe id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<RecipeQuantityData> GetRecipeQuantityByParentId(int id)
        {
            List<RecipeQuantityData> ret = new List<RecipeQuantityData>();
            var db = DataManager.Instance.GetDatabase();
            var col = db.GetCollection<RecipeQuantityData>(CollectionLabels.RecipeQuantities);

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
            var db = DataManager.Instance.GetDatabase();
            var col = db.GetCollection<RecipeData>(CollectionLabels.Recipes);

            return col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .FindById(id);
        }
        
    }
}
