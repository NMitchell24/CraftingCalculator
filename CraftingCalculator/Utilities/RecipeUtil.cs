using System.Collections.Generic;
using System.Linq;
using CraftingCalculator.Model.Recipes;


namespace CraftingCalculator.Utilities
{
    /// <summary>
    /// For looking up lists of recipes.  
    /// Separate Functions are provided to get lists for UI Filters.
    /// New Recipes should be added into the appropriate Get function 
    /// so that they are displayed in the proper filter.
    /// If a new Get is added for a new filter, then it should also be added 
    /// into the GetAll function so that the new recipes are included in the 'All' filter.
    /// </summary>
    public static class RecipeUtil
    {
        /// <summary>
        /// Get a list of RecipeFilters
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilter> GetRecipeFilters()
        {
            List<RecipeFilter> ret = new List<RecipeFilter>
            {
                new RecipeFilter("All", RecipeType.ALL),
                new RecipeFilter("Access Card", RecipeType.ACCESS_CARD),
                new RecipeFilter("Advanced Agricultural Product", RecipeType.ADVANCED_AGRICULTURAL_PRODUCT),
                new RecipeFilter("Advanced Crafted Product", RecipeType.ADVANCED_CRAFTED_PRODUCT),
                new RecipeFilter("Alloy Metal", RecipeType.ALLOY_METAL),
                new RecipeFilter("Atlas Seed", RecipeType.ATLAS_SEED),
                new RecipeFilter("Base Aquatic Construction", RecipeType.BASE_AQUATIC_CONSTRUCTION),
                new RecipeFilter("Base Components (Concrete)", RecipeType.BASE_COMPONENT_CONCRETE),
                new RecipeFilter("Base Components (Metal)", RecipeType.BASE_COMPONENT_METAL),
                new RecipeFilter("Base Components (Wood)", RecipeType.BASE_COMPONENT_WOOD),
                new RecipeFilter("Base Decorations", RecipeType.BASE_DECORATION),
                new RecipeFilter("Base Equipment", RecipeType.BASE_EQUIPMENT),
                new RecipeFilter("Base Storage", RecipeType.BASE_STORAGE),
                new RecipeFilter("Base Structures", RecipeType.BASE_STRUCTURE),
                new RecipeFilter("Base Terminals", RecipeType.BASE_TERMINAL),
                new RecipeFilter("Concentrated Deposits", RecipeType.CONCENTRATED_DEPOSIT),
                new RecipeFilter("Consumables", RecipeType.CONSUMABLE),
                new RecipeFilter("Crafting Components", RecipeType.CRAFTING_COMPONENT),
                new RecipeFilter("Enhanced Gas Products", RecipeType.ENHANCED_GAS_PRODUCT),
                new RecipeFilter("Enriched Alloy Metals", RecipeType.ENRICHED_ALLOY_METAL),
                new RecipeFilter("Exocraft Technology", RecipeType.EXOCRAFT_TECHNOLOGY),
                new RecipeFilter("Exocraft Terminals", RecipeType.EXOCRAFT_TERMINAL),
                new RecipeFilter("Exosuit Technology", RecipeType.EXOSUIT_TECHNOLOGY),
                new RecipeFilter("Farming", RecipeType.FARMING),
                new RecipeFilter("Freighter Technology", RecipeType.FREIGHTER_TECHNOLOGY),
                new RecipeFilter("Highly Refined Technology", RecipeType.HIGHLY_REFINED_TECHNOLOGY),
                new RecipeFilter("Manufactured Gas Products", RecipeType.MANUFACTURED_GAS_PRODUCT),
                new RecipeFilter("Multi-tool Technology", RecipeType.MULTITOOL_TECHNOLOGY),
                new RecipeFilter("Portable Technology", RecipeType.PORTABLE_TECHNOLOGY),
                new RecipeFilter("Starship Technology", RecipeType.STARSHIP_TECHNOLOGY)
            };

            return ret;
        }

        /// <summary>
        /// Return a list of Recipes for a given RecipeFilter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Recipe> GetRecipesByFilter(RecipeFilter filter)
        {
            List<Recipe> ret = SortRecipes((List<Recipe>)ReflectionUtil.
                GetEnumerableOfType<Recipe>(filter.Type.GetNamespace(), new string[0]));
            return ret;
        }

        /// <summary>
        /// Helper method to sort the list of recipes by the Recipe Name alphabetically
        /// </summary>
        /// <param name="recipes"></param>
        /// <returns></returns>
        private static List<Recipe> SortRecipes(List<Recipe> recipes)
        {
            return recipes.OrderBy(o => o.Name).ToList();
        }
    }
}
