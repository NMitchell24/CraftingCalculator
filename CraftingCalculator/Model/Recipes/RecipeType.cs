using System;
using System.Reflection;

namespace CraftingCalculator.Model.Recipes
{
    class RecipeAttr : Attribute
    {
        internal RecipeAttr(string nspace)
        {
            this.Nspace = nspace;
        }
        public string Nspace { get; private set; }
    }

    public static class RecipeExtensions
    {
        public static string GetNamespace(this RecipeType r)
        {
            RecipeAttr attr = GetAttr(r);
            return attr.Nspace;
        }

        private static RecipeAttr GetAttr(RecipeType r)
        {
            return (RecipeAttr)Attribute.GetCustomAttribute(ForValue(r), typeof(RecipeAttr));
        }

        private static MemberInfo ForValue(RecipeType r)
        {
            MemberInfo ret = typeof(RecipeType).GetField(Enum.GetName(typeof(RecipeType), r));

            return ret;
        }
    }

    public enum RecipeType
    {
        [RecipeAttr(null)]                                                                  ALL,
        [RecipeAttr("CraftingCalculator.Model.Recipes.AccessCard")]                         ACCESS_CARD,
        [RecipeAttr("CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct")]        ADVANCED_AGRICULTURAL_PRODUCT,
        [RecipeAttr("CraftingCalculator.Model.Recipes.AdvancedCraftedProduct")]             ADVANCED_CRAFTED_PRODUCT,
        [RecipeAttr("CraftingCalculator.Model.Recipes.AlloyMetal")]                         ALLOY_METAL,
        [RecipeAttr("CraftingCalculator.Model.Recipes.AtlasSeed")]                          ATLAS_SEED,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseAquaticConstruction")]            BASE_AQUATIC_CONSTRUCTION,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete")]        BASE_COMPONENT_CONCRETE,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal")]           BASE_COMPONENT_METAL,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseBasicComponentsWood")]            BASE_COMPONENT_WOOD,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseDecoration")]                     BASE_DECORATION,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseStorage")]                        BASE_STORAGE,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseEquipment")]                      BASE_EQUIPMENT,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseStructures")]                     BASE_STRUCTURE,
        [RecipeAttr("CraftingCalculator.Model.Recipes.BaseTerminals")]                      BASE_TERMINAL,
        [RecipeAttr("CraftingCalculator.Model.Recipes.ConcentratedDeposit")]                CONCENTRATED_DEPOSIT,
        [RecipeAttr("CraftingCalculator.Model.Recipes.Consumable")]                         CONSUMABLE,
        [RecipeAttr("CraftingCalculator.Model.Recipes.CraftingComponent")]                  CRAFTING_COMPONENT,
        [RecipeAttr("CraftingCalculator.Model.Recipes.EnhancedGasProduct")]                 ENHANCED_GAS_PRODUCT,
        [RecipeAttr("CraftingCalculator.Model.Recipes.EnrichedAlloyMetal")]                 ENRICHED_ALLOY_METAL,
        [RecipeAttr("CraftingCalculator.Model.Recipes.ExocraftTechnology")]                 EXOCRAFT_TECHNOLOGY,
        [RecipeAttr("CraftingCalculator.Model.Recipes.ExocraftTerminal")]                   EXOCRAFT_TERMINAL,
        [RecipeAttr("CraftingCalculator.Model.Recipes.ExosuitTechnology")]                  EXOSUIT_TECHNOLOGY,
        [RecipeAttr("CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair")]            EXOSUIT_TECHNOLOGY_REPAIR,
        [RecipeAttr("CraftingCalculator.Model.Recipes.Farming")]                            FARMING,
        [RecipeAttr("CraftingCalculator.Model.Recipes.FreighterTechnology")]                FREIGHTER_TECHNOLOGY,
        [RecipeAttr("CraftingCalculator.Model.Recipes.HighlyRefinedTechnology")]            HIGHLY_REFINED_TECHNOLOGY,
        [RecipeAttr("CraftingCalculator.Model.Recipes.ManufacturedGasProduct")]             MANUFACTURED_GAS_PRODUCT,
        [RecipeAttr("CraftingCalculator.Model.Recipes.MultitoolTechnology")]                MULTITOOL_TECHNOLOGY,
        [RecipeAttr("CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair")]          MULTITOOL_TECH_REPAIR,
        [RecipeAttr("CraftingCalculator.Model.Recipes.PortableTechnology")]                 PORTABLE_TECHNOLOGY,
        [RecipeAttr("CraftingCalculator.Model.Recipes.StarshipTechnology")]                 STARSHIP_TECHNOLOGY,
        [RecipeAttr("CraftingCalculator.Model.Recipes.StarshipTechnologyRepair")]           STARSHIP_TECHNOLOGY_REPAIR,
    }
}
