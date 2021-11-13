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
            Recipe ret = default!;
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
                Name = data.Name ?? "",
                Description = data.Description ?? "",
                Value = data.Value,
                Id = data.Id
            };

            if (data.Filter != null)
            {
                RecipeFilter filter = RecipeFilterService.GetRecipeFilterById(data.Filter.Id);
                ret.Filter = filter;
            }

            IngredientMap ingMap = new IngredientMap();
            if (data.Ingredients != null)
            {
                foreach (IngredientQuantityData iData in data.Ingredients)
                {
                    IngredientQuantityData fullData = IngredientDAO.GetIngredientQuantityById(iData.Id);
                    if (fullData != null && fullData.Ingredient != null)
                    {
                        Ingredient ing = IngredientService.GetIngredientById(fullData.Ingredient.Id);

                        ingMap.Add(ing, fullData.Quantity, fullData.Id);
                    }
                }
            }
            ret.Ingredients = ingMap;

            List<RecipeQuantityData> children = RecipeDAO.GetRecipeQuantityByParentId(data.Id);

            if (children != null && children.Count > 0)
            {
                RecipeMap recMap = new RecipeMap();
                foreach (RecipeQuantityData recQ in children)
                {
                    if (recQ.ChildRecipe != null)
                    {
                        RecipeData childData = RecipeDAO.GetRecipeById(recQ.ChildRecipe.Id);
                        Recipe child = GetRecipeForData(childData);
                        recMap.Add(child, recQ.Quantity, recQ.Id);
                    }
                }
                ret.ChildRecipes = recMap;
            }
            else if (children == null || children.Count == 0)
            {
                ret.ChildRecipes = new RecipeMap();
            }

            return ret;
        }

        /// <summary>
        /// Returns a list of all Recipes from the database.
        /// </summary>
        /// <returns></returns>
        public static List<Recipe> GetAllRecipes()
        {
            return GetRecipeListForDataList(RecipeDAO.GetAllRecipeData());
        }

        /// <summary>
        /// updates an existing recipe or adds a new one
        /// </summary>
        /// <param name="recipe"></param>
        public static void SaveRecipe(Recipe? recipe)
        {
            if (recipe != null)
            {
                RecipeData data = new RecipeData();
                if (recipe.Id > 0)
                {
                    data = RecipeDAO.GetRecipeById(recipe.Id);
                }
                else
                {
                    data.Ingredients = new List<IngredientQuantityData>();
                }
                //Set all basic fields.
                data.Name = recipe.Name ?? "";
                data.Description = recipe.Description ?? "";
                data.Value = recipe.Value;

                //Set recipe filter
                if (recipe.Filter != null)
                {
                    RecipeFilterData filterData = AbstractDAO.GetRecordById<RecipeFilterData>(CollectionLabels.RecipeFilters, recipe.Filter.Id);
                    data.Filter = filterData;
                }

                //Check and remove any IngredientQuantities
                if (recipe.Ingredients.RemovedIngredients.Count > 0)
                {
                    foreach (IngredientQuantity iq in recipe.Ingredients.RemovedIngredients)
                    {
                        if (iq.Id > 0)
                        {
                            AbstractDAO.DeleteRecordById<IngredientQuantityData>(CollectionLabels.IngredientQuantities, iq.Id);
                            //Also remove the record from the recipe data list if it exists.
                            if (data.Ingredients != null)
                            {
                                data.Ingredients.RemoveAll(x => x.Id == iq.Id);
                            }
                        }
                    }
                }
                //Add or update any IngredientQuantities
                foreach (IngredientQuantity iq in recipe.Ingredients.IngredientList)
                {
                    if (iq.Id > 0)
                    {
                        IngredientQuantityData iqData = IngredientDAO.GetIngredientQuantityById(iq.Id);
                        iqData.Quantity = iq.Quantity;
                        AbstractDAO.AddOrUpdateRecord<IngredientQuantityData>(CollectionLabels.IngredientQuantities, iqData);
                    }
                    else
                    {
                        IngredientData ing = AbstractDAO.GetRecordById<IngredientData>(CollectionLabels.Ingredients, iq.Ingredient.Id);
                        IngredientQuantityData iqData = new IngredientQuantityData
                        {
                            Ingredient = ing,
                            Quantity = iq.Quantity
                        };
                        //Add the record in the DB
                        AbstractDAO.AddOrUpdateRecord<IngredientQuantityData>(CollectionLabels.IngredientQuantities, iqData);
                        if (data.Ingredients != null)
                        {
                            //Now update RecipeData
                            data.Ingredients.Add(iqData);
                        }
                    }
                }

                //Save the recipe before modifying ChildRecipes as child recipes are dependent on the parent recipe ID already existing.
                AbstractDAO.AddOrUpdateRecord<RecipeData>(CollectionLabels.Recipes, data);

                //Check and remove any ChildRecipes
                foreach (RecipeQuantity rq in recipe.ChildRecipes.RemovedRecipes)
                {
                    if (rq.Id > 0)
                    {
                        AbstractDAO.DeleteRecordById<RecipeQuantityData>(CollectionLabels.RecipeQuantities, rq.Id);
                    }
                }
                //Add or update any ChildRecipes
                foreach (RecipeQuantity rq in recipe.ChildRecipes.RecipeList)
                {
                    RecipeQuantityData rqData = new RecipeQuantityData();
                    if (rq.Id > 0)
                    {
                        rqData = RecipeDAO.GetRecipeQuantityByQuantityId(rq.Id);
                    }
                    else
                    {
                        RecipeData childRecipe = RecipeDAO.GetRecipeById(rq.Recipe.Id);
                        rqData.ParentRecipe = data;
                        rqData.ChildRecipe = childRecipe;
                    }
                    rqData.Quantity = rq.Quantity;
                    AbstractDAO.AddOrUpdateRecord<RecipeQuantityData>(CollectionLabels.RecipeQuantities, rqData);
                }
            }
        }

        /// <summary>
        /// Deletes the provided Recipe
        /// </summary>
        /// <param name="recipe"></param>
        public static void DeleteRecipe(Recipe? recipe)
        {
            if (recipe != null)
            {
                //First delete from any Recipe Favorites
                List<FavoriteRecipeQuantitiesData> recipeFavs = RecipeFavoritesDAO.GetFavoriteQuantitiesByRecipeId(recipe.Id);
                foreach (FavoriteRecipeQuantitiesData favData in recipeFavs)
                {
                    AbstractDAO.DeleteRecordById<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities, favData.Id);
                }
                //Delete any child recipe quantities
                List<RecipeQuantityData> childRecipes = RecipeDAO.GetRecipeQuantityByParentId(recipe.Id);
                foreach (RecipeQuantityData recipeQuantity in childRecipes)
                {
                    AbstractDAO.DeleteRecordById<RecipeQuantityData>(CollectionLabels.RecipeQuantities, recipeQuantity.Id);
                }
                //Delete any Recipe Quantities where this recipe is used as the Child Recipe
                List<RecipeQuantityData> parentRecipes = RecipeDAO.GetRecipeQuantityByChildId(recipe.Id);
                foreach (RecipeQuantityData recipeQuantity in parentRecipes)
                {
                    AbstractDAO.DeleteRecordById<RecipeQuantityData>(CollectionLabels.RecipeQuantities, recipeQuantity.Id);
                }
                //Delete any Ingredient Quantities
                foreach (IngredientQuantity iq in recipe.Ingredients.IngredientList)
                {
                    AbstractDAO.DeleteRecordById<IngredientQuantityData>(CollectionLabels.IngredientQuantities, iq.Id);
                }
                //Delete the Recipe
                AbstractDAO.DeleteRecordById<RecipeData>(CollectionLabels.Recipes, recipe.Id);
            }
        }
    }
}
