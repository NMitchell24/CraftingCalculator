using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CraftingCalculator.Recipes;
using CraftingCalculator.Recipes.AccessCard;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Recipes.AdvancedCraftedProduct;
using CraftingCalculator.Recipes.AlloyMetal;
using CraftingCalculator.Recipes.AtlasSeed;
using CraftingCalculator.Recipes.ConcentratedDeposit;
using CraftingCalculator.Recipes.Consumable;
using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.EnhancedGasProduct;
using CraftingCalculator.Recipes.EnrichedAlloyMetal;
using CraftingCalculator.Recipes.ExocraftTechnology;
using CraftingCalculator.Recipes.ExosuitTechnology;
using CraftingCalculator.Recipes.Farming;
using CraftingCalculator.Recipes.HighlyRefinedTechnology;
using CraftingCalculator.Recipes.ManufacturedGasProduct;
using CraftingCalculator.Recipes.MultitoolTechnology;
using CraftingCalculator.Recipes.StarshipTechnology;

namespace CraftingCalculator.Utilities
{
    public static class RecipeUtil
    {
        public static List<Recipe> GetAccessCardRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AtlasPass1(),
                new AtlasPass2(),
                new AtlasPass3()
            };
            return ret;
        }

        public static List<Recipe> GetAdvancedAgriculturalProductRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                //TODO: new Recipe(), etc...
            };
            return ret;
        }


        public static List<Recipe> GetAllRecipes()
        {
            List<Recipe> ret = new List<Recipe>();
            ret.AddRange(GetAccessCardRecipes());
            ret.AddRange(GetAdvancedAgriculturalProductRecipes());
            //ETC, ETC, ETC....

            return ret;
        }
    }
}
