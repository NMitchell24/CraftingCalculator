using System;
using System.Reflection;

namespace CraftingCalculator.Ingredients
{
    class IngredientAttr : Attribute
    {
        internal IngredientAttr(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }

    public static class IngredientExtensions
    {
        public static string GetDisplayName(this IngredientType i)
        {
            IngredientAttr attr = GetAttr(i);
            return attr.Name;
        }

        private static IngredientAttr GetAttr(IngredientType i)
        {
            return (IngredientAttr)Attribute.GetCustomAttribute(ForValue(i), typeof(IngredientAttr));
        }

        private static MemberInfo ForValue(IngredientType i)
        {
            return typeof(IngredientType).GetField(Enum.GetName(typeof(IngredientType), i));
        }
    }

    public enum IngredientType
    {
        [IngredientAttr("Carbon")]              CARBON,
        [IngredientAttr("Condensed Carbon")]    CONDENSED_CARBON,
        [IngredientAttr("Oxygen")]              OXYGEN,
        [IngredientAttr("Hexite")]              HEXITE,
        [IngredientAttr("Di-Hydrogen")]         DIHYDROGEN,
        [IngredientAttr("Deuterium")]           DEUTERIUM,
        [IngredientAttr("Tritium")]             TRITIUM,
        [IngredientAttr("Ferrite Dust")]        FERRITE_DUST,
        [IngredientAttr("Pure Ferrite")]        PURE_FERRITE,
        [IngredientAttr("Magnetised Ferrite")]  MAGNETISED_FERRITE,
        [IngredientAttr("Sodium")]              SODIUM,
        [IngredientAttr("Sodium Nitrate")]      SODIUM_NITRATE,
        [IngredientAttr("Cobalt")]              COBALT,
        [IngredientAttr("Ionised Cobalt")]      IONISED_COBALT,
        [IngredientAttr("Salt")]                SALT,
        [IngredientAttr("Chlorine")]            CHLORINE,
        [IngredientAttr("Cyto-Phosphate")]      CYTOPHOSPHATE,
        [IngredientAttr("Copper")]              COPPER,
        [IngredientAttr("Cadmium")]             CADMIUM,
        [IngredientAttr("Emeril")]              EMERIL,
        [IngredientAttr("Indum")]               INDIUM,
        [IngredientAttr("Chromatic Metal")]     CHROMATIC_METAL,
        [IngredientAttr("Paraffinium")]         PARAFFINIUM,
        [IngredientAttr("Pyrite")]              PYRITE,
        [IngredientAttr("Ammonia")]             AMMONIA,
        [IngredientAttr("Uranium")]             URANIUM,
        [IngredientAttr("Dioxite")]             DIOXITE,
        [IngredientAttr("Phosphorus")]          PHOSPHORUS,
        [IngredientAttr("Mordite")]             MORDITE,
        [IngredientAttr("Pugneum")]             PUGNEUM,
        [IngredientAttr("Silver")]              SILVER,
        [IngredientAttr("Gold")]                GOLD,
        [IngredientAttr("Platinum")]            PLATINUM,
        [IngredientAttr("Sulphurine")]          SULPHURINE,
        [IngredientAttr("Radon")]               RADON,
        [IngredientAttr("Nitrogen")]            NITROGEN,
        [IngredientAttr("Activated Copper")]    ACTIVATED_COPPER,
        [IngredientAttr("Activated Cadmium")]   ACTIVATED_CADMIUM,
        [IngredientAttr("Activated Emeril")]    ACTIVATED_EMERIL,
        [IngredientAttr("Activated Indum")]     ACTIVATED_INDUM,
        [IngredientAttr("Fungal Mould")]        FUNGAL_MOULD,
        [IngredientAttr("Frost Crystal")]       FROST_CRYSTAL,
        [IngredientAttr("Gamma Root")]          GAMMA_ROOT,
        [IngredientAttr("Cactus Flesh")]        CACTUS_FLESH,
        [IngredientAttr("Solanium")]            SOLANIUM,
        [IngredientAttr("Star Bulb")]           STAR_BULB,
        [IngredientAttr("Marrow Bulb")]         MARROW_BULB,
        [IngredientAttr("Kelp Sac")]            KELP_SAC,
        [IngredientAttr("Coprite")]             COPRITE,
        [IngredientAttr("Residual Goop")]       RESIDUAL_GOOP,
        [IngredientAttr("Runaway Mould")]       RUNAWAY_MOULD,
        [IngredientAttr("Rusted Metal")]        RUSTED_METAL,
        [IngredientAttr("Living Slime")]        LIVING_SLIME,
        [IngredientAttr("Viscious Fluids")]     VISCIOUS_FLUIDS,
        [IngredientAttr("Technology Module")]   TECHNOLOGY_MODULE,
        [IngredientAttr("Quad Servo")]          QUAD_SERVO,
        [IngredientAttr("Walker Brain")]        WALKER_BRAIN
    }

}
