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
        // I'm using Reflection and Assembly.GetTypes to build these.  Don't want to rebuild every time, so store them
        // in a Dictionary once they've been built and read from the Dictionary first if it exists.
        private static IDictionary<RecipeType, List<Recipe>> recipeCache = new Dictionary<RecipeType, List<Recipe>>();
        
        /// <summary>
        /// Get a list of RecipeFilters
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilter> GetRecipeFilters()
        {
            List<RecipeFilter> ret = new List<RecipeFilter>
            {
                new RecipeFilter(RecipeFilterLabels.All, RecipeType.ALL),
                new RecipeFilter(RecipeFilterLabels.AccessCard, RecipeType.ACCESS_CARD),
                new RecipeFilter(RecipeFilterLabels.AdvancedAgriculturalProduct, RecipeType.ADVANCED_AGRICULTURAL_PRODUCT),
                new RecipeFilter(RecipeFilterLabels.AdvancedCraftedProduct, RecipeType.ADVANCED_CRAFTED_PRODUCT),
                new RecipeFilter(RecipeFilterLabels.AlloyMetal, RecipeType.ALLOY_METAL),
                new RecipeFilter(RecipeFilterLabels.AtlasSeed, RecipeType.ATLAS_SEED),
                new RecipeFilter(RecipeFilterLabels.BaseAquaticConstruction, RecipeType.BASE_AQUATIC_CONSTRUCTION),
                new RecipeFilter(RecipeFilterLabels.BaseComponentsConcrete, RecipeType.BASE_COMPONENT_CONCRETE),
                new RecipeFilter(RecipeFilterLabels.BaseComponentsMetal, RecipeType.BASE_COMPONENT_METAL),
                new RecipeFilter(RecipeFilterLabels.BaseComponentsWood, RecipeType.BASE_COMPONENT_WOOD),
                new RecipeFilter(RecipeFilterLabels.BaseDecorations, RecipeType.BASE_DECORATION),
                new RecipeFilter(RecipeFilterLabels.BaseEquipment, RecipeType.BASE_EQUIPMENT),
                new RecipeFilter(RecipeFilterLabels.BaseStorage, RecipeType.BASE_STORAGE),
                new RecipeFilter(RecipeFilterLabels.BaseStructures, RecipeType.BASE_STRUCTURE),
                new RecipeFilter(RecipeFilterLabels.BaseTerminals, RecipeType.BASE_TERMINAL),
                new RecipeFilter(RecipeFilterLabels.ConcentratedDeposits, RecipeType.CONCENTRATED_DEPOSIT),
                new RecipeFilter(RecipeFilterLabels.Consumables, RecipeType.CONSUMABLE),
                new RecipeFilter(RecipeFilterLabels.CraftingComponents, RecipeType.CRAFTING_COMPONENT),
                new RecipeFilter(RecipeFilterLabels.EnhancedGasProduct, RecipeType.ENHANCED_GAS_PRODUCT),
                new RecipeFilter(RecipeFilterLabels.EnrichedAlloyMetals, RecipeType.ENRICHED_ALLOY_METAL),
                new RecipeFilter(RecipeFilterLabels.ExocraftTech, RecipeType.EXOCRAFT_TECHNOLOGY),
                new RecipeFilter(RecipeFilterLabels.ExocraftTechRepair, RecipeType.EXOCRAFT_TECHNOLOGY_REPAIR),
                new RecipeFilter(RecipeFilterLabels.ExocraftTerminals, RecipeType.EXOCRAFT_TERMINAL),
                new RecipeFilter(RecipeFilterLabels.ExosuitTech, RecipeType.EXOSUIT_TECHNOLOGY),
                new RecipeFilter(RecipeFilterLabels.ExosuitTechRepair, RecipeType.EXOSUIT_TECHNOLOGY_REPAIR),
                new RecipeFilter(RecipeFilterLabels.Farming, RecipeType.FARMING),
                new RecipeFilter(RecipeFilterLabels.FreighterTech, RecipeType.FREIGHTER_TECHNOLOGY),
                new RecipeFilter(RecipeFilterLabels.HighlyRefinedTech, RecipeType.HIGHLY_REFINED_TECHNOLOGY),
                new RecipeFilter(RecipeFilterLabels.ManufacturedGasProducts, RecipeType.MANUFACTURED_GAS_PRODUCT),
                new RecipeFilter(RecipeFilterLabels.MultitoolTech, RecipeType.MULTITOOL_TECHNOLOGY),
                new RecipeFilter(RecipeFilterLabels.MultitoolTechRepair, RecipeType.MULTITOOL_TECH_REPAIR),
                new RecipeFilter(RecipeFilterLabels.PortableTech, RecipeType.PORTABLE_TECHNOLOGY),
                new RecipeFilter(RecipeFilterLabels.StarshipTech, RecipeType.STARSHIP_TECHNOLOGY),
                new RecipeFilter(RecipeFilterLabels.StarshipTechRepair, RecipeType.STARSHIP_TECHNOLOGY_REPAIR)
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
            List<Recipe> ret;          

            if (recipeCache.ContainsKey(filter.Type))
            {
                ret = recipeCache[filter.Type];
            }
            else
            {
                ret = SortRecipes((List<Recipe>)ReflectionUtil.
                    GetEnumerableOfType<Recipe>(filter.Type.GetNamespace(), new string[0]));
                recipeCache.Add(filter.Type, ret);
            }
            
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
