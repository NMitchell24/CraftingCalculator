using CraftingCalculator.Model.LiteDB;
using LiteDB;
using System;
using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes;

namespace CraftingCalculator.Utilities
{
    public static class DataUtil
    {
        
        public static List<RecipeFilterData> GetAllRecipeFiltersData()
        {
            var db = DataManager.Instance.GetDatabase();
            var filters = db.GetCollection<RecipeFilterData>(CollectionLabels.RecipeFilters);
            List<RecipeFilterData> ret = new List<RecipeFilterData>();

            var all = filters.Find(Query.All(Query.Ascending));
            ret.AddRange(all);
            return ret;
        }


        //Temporarily keeping this in case I need it again.  
        //TODO: Remove before final commit.
        public static void CreateDatabase()
        {
            LiteDatabase db = DataManager.Instance.GetDatabase();
            Dictionary<RecipeData, List<RecipeQuantity>> childRecipeMap = new Dictionary<RecipeData, List<RecipeQuantity>>();
            //Create Collections:
            var ingredients = db.GetCollection<IngredientData>(CollectionLabels.Ingredients);
            var recipeFilters = db.GetCollection<RecipeFilterData>(CollectionLabels.RecipeFilters);
            var ingredientQuantites = db.GetCollection<IngredientQuantityData>(CollectionLabels.IngredientQuantities);
            var recipeQuantities = db.GetCollection<RecipeQuantityData>(CollectionLabels.RecipeQuantities);
            var recipes = db.GetCollection<RecipeData>(CollectionLabels.Recipes);

            //Create Ingredients
            var Ings = Enum.GetValues(typeof(IngredientType));
            foreach (var i in Ings)
            {
                var ing = new IngredientData
                {
                    Name = ((IngredientType)i).GetDisplayName(),
                    Description = ((IngredientType)i).GetDisplayName()
                };
                ingredients.Insert(ing);
            }
            //build index for Ingredient Name
            ingredients.EnsureIndex(x => x.Name);


            //Create RecipeFilters
            foreach (var r in RecipeUtil.GetRecipeFilters())
            {
                RecipeFilterData filter = new RecipeFilterData
                {
                    Name = r.Name
                };

                recipeFilters.Insert(filter);
            }
            //Build index for RecipeFilter name
            recipeFilters.EnsureIndex(x => x.Name);

            //Create Recipes
            int id = 1;
            int id2 = 1;
            foreach (Recipe rec in RecipeUtil.GetRecipesByFilter(new RecipeFilter(RecipeFilterLabels.All, RecipeType.ALL)))
            {
                RecipeData data = new RecipeData
                {
                    Id = id,
                    Name = rec.Name,
                    Description = rec.Name,
                    Ingredients = new List<IngredientQuantityData>()
                };


                //Lookup filter and set it on the recipe
                var filter = recipeFilters.FindOne(x => x.Name == rec.Type);
                if (filter != null)
                {
                    data.Filter = filter;
                }
                else
                {
                    throw new ArgumentNullException("RecipeFilter cannot be null for Recipe " + rec.Name + ", " + rec.Type);
                }

                //IngredientQuantities
                foreach (var i in rec.GetBaseIngredients().IngredientList)
                {
                    //Lookup the ingredient object
                    var IngD = ingredients.FindOne(x => x.Name == i.Name);
                    if (IngD != null)
                    {
                        IngredientQuantityData IngQuantD = new IngredientQuantityData
                        {
                            Id = id2,
                            Ingredient = IngD,
                            Quantity = i.Quantity
                        };

                        ingredientQuantites.Insert(IngQuantD);

                        data.Ingredients.Add(ingredientQuantites.FindById(id2));

                        id2++;
                    }
                    else
                    {
                        throw new ArgumentNullException("Ingredient For Recipe cannot be null " + rec.Name + ", " + i.Name);
                    }
                }

                recipes.Insert(data);

                //If this is a complex recipe store child recipes in a map temporarily.  This will be processed after all recipes are finished.
                if (rec.GetType().IsSubclassOf(typeof(ComplexRecipe)))
                {
                    List<RecipeQuantity> childRecipes = new List<RecipeQuantity>();

                    foreach (RecipeQuantity recQ in ((ComplexRecipe)rec).GetChildren().RecipeList)
                    {
                        childRecipes.Add(recQ);
                    }

                    childRecipeMap.Add(recipes.FindById(id), childRecipes);

                }

                id++;
            }

            //RecipeQuantities
            foreach (KeyValuePair<RecipeData, List<RecipeQuantity>> c in childRecipeMap)
            {
                RecipeData parent = c.Key;
                foreach (RecipeQuantity rq in c.Value)
                {
                    RecipeData child = recipes.FindOne(x => x.Name == rq.Recipe.Name);

                    if (child != null)
                    {
                        RecipeQuantityData rqData = new RecipeQuantityData
                        {
                            ParentRecipe = parent,
                            ChildRecipe = child,
                            Quantity = rq.Quantity
                        };

                        recipeQuantities.Insert(rqData);
                    }
                    else
                    {
                        throw new ArgumentNullException("ChildRecipe For RecipeQuantity cannot be null " + rq.Recipe.Name + ", " + c.Key.Name);
                    }

                }
            }

        }

    }
}
