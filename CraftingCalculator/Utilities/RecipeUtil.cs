using System.Collections.Generic;
using CraftingCalculator.Model.Recipes;
using CraftingCalculator.Model.Recipes.AccessCard;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Model.Recipes.AdvancedCraftedProduct;
using CraftingCalculator.Model.Recipes.AlloyMetal;
using CraftingCalculator.Model.Recipes.AtlasSeed;
using CraftingCalculator.Model.Recipes.BaseAquaticConstruction;
using CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete;
using CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal;
using CraftingCalculator.Model.Recipes.BaseBasicComponentsWood;
using CraftingCalculator.Model.Recipes.BaseDecoration;
using CraftingCalculator.Model.Recipes.BaseEquipment;
using CraftingCalculator.Model.Recipes.BaseStorage;
using CraftingCalculator.Model.Recipes.BaseStructures;
using CraftingCalculator.Model.Recipes.BaseTerminals;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;
using CraftingCalculator.Model.Recipes.Consumable;
using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Recipes.EnhancedGasProduct;
using CraftingCalculator.Model.Recipes.EnrichedAlloyMetal;
using CraftingCalculator.Model.Recipes.ExocraftTechnology;
using CraftingCalculator.Model.Recipes.ExocraftTerminal;
using CraftingCalculator.Model.Recipes.ExosuitTechnology;
using CraftingCalculator.Model.Recipes.Farming;
using CraftingCalculator.Model.Recipes.FreighterTechnology;
using CraftingCalculator.Model.Recipes.HighlyRefinedTechnology;
using CraftingCalculator.Model.Recipes.ManufacturedGasProduct;
using CraftingCalculator.Model.Recipes.MultitoolTechnology;
using CraftingCalculator.Model.Recipes.PortableTechnology;
using CraftingCalculator.Model.Recipes.StarshipTechnology;

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
            return GetRecipesByType(filter.Type);
        }

        /// <summary>
        /// Get a list of Recipes for a specified RecipeType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Recipe> GetRecipesByType(RecipeType type)
        {
            switch (type)
            {
                case RecipeType.ALL:
                    return GetAllRecipes();
                case RecipeType.ACCESS_CARD:
                    return GetAccessCardRecipes();
                case RecipeType.ADVANCED_AGRICULTURAL_PRODUCT:
                    return GetAdvancedAgriculturalProductRecipes();
                case RecipeType.ADVANCED_CRAFTED_PRODUCT:
                    return GetAdvancedCraftedProductRecipes();
                case RecipeType.ALLOY_METAL:
                    return GetAlloyMetalRecipes();
                case RecipeType.ATLAS_SEED:
                    return GetAtlasSeedRecipes();
                case RecipeType.BASE_AQUATIC_CONSTRUCTION:
                    return GetBaseAquaticConstructionRecipes();
                case RecipeType.BASE_COMPONENT_CONCRETE:
                    return GetBaseConcreteComponenetsRecipes();
                case RecipeType.BASE_COMPONENT_METAL:
                    return GetBaseMetalComponentRecipes();
                case RecipeType.BASE_COMPONENT_WOOD:
                    return GetBaseWoodComponentRecipes();
                case RecipeType.BASE_DECORATION:
                    return GetBaseDecorationRecipes();
                case RecipeType.BASE_EQUIPMENT:
                    return GetBaseEquipmentRecipes();
                case RecipeType.BASE_STRUCTURE:
                    return GetBaseStructureRecipes();
                case RecipeType.BASE_TERMINAL:
                    return GetBaseTerminalRecipes();
                case RecipeType.CONCENTRATED_DEPOSIT:
                    return GetConcentratedDepositRecipes();
                case RecipeType.CONSUMABLE:
                    return GetConsumableRecipes();
                case RecipeType.CRAFTING_COMPONENT:
                    return GetCraftingComponentRecipes();
                case RecipeType.ENHANCED_GAS_PRODUCT:
                    return GetEnhancedGasProductRecipes();
                case RecipeType.ENRICHED_ALLOY_METAL:
                    return GetEnrichedAlloyMetalRecipes();
                case RecipeType.EXOCRAFT_TECHNOLOGY:
                    return GetExocraftTechnologyRecipes();
                case RecipeType.EXOCRAFT_TERMINAL:
                    return GetExocraftTerminalRecipes();
                case RecipeType.EXOSUIT_TECHNOLOGY:
                    return GetExosuitTechnologyRecipes();
                case RecipeType.FARMING:
                    return GetFarmingRecipes();
                case RecipeType.FREIGHTER_TECHNOLOGY:
                    return GetFreighterTechnologyRecipes();
                case RecipeType.HIGHLY_REFINED_TECHNOLOGY:
                    return GetHighlyRefinedTechnologyRecipes();
                case RecipeType.MANUFACTURED_GAS_PRODUCT:
                    return GetManufacturedGasProductRecipes();
                case RecipeType.MULTITOOL_TECHNOLOGY:
                    return GetMultitoolTechnologyRecipes();
                case RecipeType.PORTABLE_TECHNOLOGY:
                    return GetPortableTechnologyRecipes();
                case RecipeType.STARSHIP_TECHNOLOGY:
                    return GetStarshipTechnologyRecipes();
                default:
                    return GetAllRecipes();
            }
        }

        private static List<Recipe> GetAccessCardRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AtlasPass1(),
                new AtlasPass2(),
                new AtlasPass3()
            };
            return ret;
        }

        private static List<Recipe> GetAdvancedAgriculturalProductRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new Acid(),
                new CircuitBoard(),
                new Glass(),
                new HeatCapacitor(),
                new LiquidExplosive(),
                new LivingGlass(),
                new Lubricant(),
                new PolyFibre(),
                new UnstableGel()
            };
            return ret;
        }

        private static List<Recipe> GetAdvancedCraftedProductRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new CryogenicChamber(),
                new MindArc(),
                new PortableReactor(),
                new QuantumProcessor()
            };
            return ret;
        }

        private static List<Recipe> GetAlloyMetalRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new Aronium(),
                new DirtyBronze(),
                new Grantine(),
                new Herox(),
                new Lemmium(),
                new MagnoGold()
            };
            return ret;
        }

        private static List<Recipe> GetAtlasSeedRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new CapturedNanode(),
                new DarkMatter(),
                new DawnsEnd(),
                new EnglobedShade(),
                new HeartOfTheSun(),
                new ModifiedQuanta(),
                new NoosphericOrb(),
                new NovaeReclaiment(),
                new PhoticJade(),
                new StatePhasure(),
            };
            return ret;
        }

        private static List<Recipe> GetBaseAquaticConstructionRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new DeepwaterChamber(),
                new GlassTunnel(),
                new LShapedGlassTunnel(),
                new MoonpoolFloor(),
                new SquareDeepwaterChamber(),
                new TShapedGlassTunnel(),
                new VerticalGlassTunnel(),
                new WatertightDoor(),
                new XShapedGlassTunnel()
            };
            return ret;
        }

        private static List<Recipe> GetBaseConcreteComponenetsRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new ConcreteArch(),
                new ConcreteDoorFrame(),
                new ConcreteDoorway(),
                new ConcreteFloorPanel(),
                new ConcreteFramedGlassPanel(),
                new ConcreteFrontage(),
                new ConcreteHalfArch(),
                new ConcreteHalfRamp(),
                new ConcreteRamp(),
                new ConcreteRoof(),
                new ConcreteRoofCorner(),
                new ConcreteRoofPanel(),
                new ConcreteWall(),
                new ConcreteWindowPanel(),
                new ShortConcreteWall(),
                new SlopingConcretePanel(),
                new SmallConcretePanel(),
                new SmallConcreteWall(),
                new ThinConcreteWall()

            };
            return ret;
        }

        private static List<Recipe> GetBaseMetalComponentRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new MetalArch(),
                new MetalDoorFrame(),
                new MetalDoorPanel(),
                new MetalDoorway(),
                new MetalFramedGlassPanel(),
                new MetalFrontage(),
                new MetalHalfArch(),
                new MetalHalfRamp(),
                new MetalRamp(),
                new MetalRoof(),
                new MetalRoofCorner(),
                new MetalRoofPanel(),
                new MetalWall(),
                new MetalWindowPanel(),
                new ShortMetalWall(),
                new SlopingMetalPanel(),
                new SmallMetalPanel(),
                new SmallMetalWall(),
                new ThinMetalWall()
            };
            return ret;
        }

        private static List<Recipe> GetBaseWoodComponentRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new ShortWoodenWall(),
                new SlopingWoodPanel(),
                new SmallWoodenWall(),
                new SmallWoodPanel(),
                new ThinWoodenWall(),
                new WoodenArch(),
                new WoodenDoorFrame(),
                new WoodenDoorway(),
                new WoodenFrontage(),
                new WoodenHalfArch(),
                new WoodenHalfRamp(),
                new WoodenRamp(),
                new WoodenRoof(),
                new WoodenRoofCorner(),
                new WoodenRoofPanel(),
                new WoodenWall(),
                new WoodenWindowPanel(),
                new WoodFloorPanel(),
                new WoodFramedGlassPanel()
            };
            return ret;
        }

        private static List<Recipe> GetBaseDecorationRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new Bed(),
                new CanisterRack(),
                new CeilingLight(),
                new CeilingPanel(),
                new Chair(),
                new ColouredLight(),
                new ColouredWallScreen(),
                new CornerSofa(),
                new Cube(),
                new CuboidRoofCap(),
                new CurvedDesk(),
                new CurvedPipe(),
                new Cylinder(),
                new Decal(),
                new Drawers(),
                new Flag(),
                new FlatPanel(), 
                new FloorMat(),
                new LabLamp(),
                new LargeMonitorStation(),
                new LargeWedge(),
                new LightTable(), 
                new Locker(),
                new MonitorStation(),
                new OctaCabinet(), 
                new Oscilloscope(),
                new PavingDecoration(),
                new Pipe(),
                new Pyramid(),
                new RoboticArm(),
                new Server(),
                new ShelfPanel(),
                new SidePanel(),
                new SimpleDesk(),
                new SmallLanternLight(),
                new SmallWedge(),
                new Sofa(),
                new Sphere(),
                new StandingLight(),
                new StoragePanel(),
                new SweptSofa(),
                new Table(),
                new TallCabinet(),
                new TechPanel(),
                new WallFan(),
                new WallFlag(),
                new WallScreen(),
                new WallUnit(),
                new WeaponRack(),
                new Worktop()
            };
            return ret;
        }

        private static List<Recipe> GetBaseEquipmentRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new BaseSalvageCapsule(),
                new BaseTeleportModule(),
                new GalacticTradeTerminal(),
                new HazardProtectionUnit(),
                new HealthStation(),
                new LandingPad(),
                new LargeRefiner()
            };
            return ret;
        }

        private static List<Recipe> GetBaseStorageRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new BarrelFabricator(),
                new CrateFabricator(),
                new LockedCrate(),
                new SmallCrate(),
                new StorageContainer()
            };
            return ret;
        }

        private static List<Recipe> GetBaseStructureRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AccessRamp(),
                new BaseLargeGlassPanel(),
                new BaseWindowLargeSquare(),
                new BaseWindowSectionedRect(),
                new BaseWindowSmallPortal(),
                new BaseWindowSmallRectangle(),
                new BioDome(),
                new CuboidInnerDoor(),
                new CuboidInnerWall(),
                new CuboidRoom(),
                new CuboidRoomFlooring(),
                new CuboidRoomFoundationStrut(),
                new CuboidRoomFoundationStrutQuad(),
                new CuboidRoomFrame(),
                new CurvedCorridor(),
                new CurvedCuboidRoof(),
                new CurvedCuboidWall(),
                new CylindricalRoom(),
                new CylindricalRoomFrame(),
                new Door(),
                new Foundation(),
                new GlassCuboidRoom(),
                new GlassRoofedCorridor(),
                new HoloDoor(),
                new InfrastructureLadder(),
                new InteriorStairs(),
                new Ladder(),
                new LShapedCorridor(),
                new Paving(),
                new SolidCube(),
                new SquareRoom(),
                new StraightCorridor(),
                new TShapedCorridor(),
                new ViewingSphere(),
                new XShapedCorridor()
            };
            return ret;
        }

        private static List<Recipe> GetBaseTerminalRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AgriculturalTerminal(),
                new ConstructionTerminal(),
                new ExocraftTerminal(),
                new ScienceTerminal(),
                new WeaponsTerminal()
            };
            return ret;
        }

        private static List<Recipe> GetConcentratedDepositRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new CarbonCrystal(),
                new ChlorideLattice(),
                new DestabalisedSodium(),
                new RareMetalElement(),
                new SuperoxideCrystal(),
                new TetraCobalt()
            };
            return ret;
        }

        private static List<Recipe> GetConsumableRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AdvancedIonBattery(),
                new ExplosiveDrones(),
                new FrigateFuel50(),
                new FrigateFuel100(),
                new FrigateFuel200(),
                new FuelOxidiser(),
                new HolographicAnalyser(),
                new HydrothermalFuelCell(),
                new IonBattery(),
                new LifeSupportGel(),
                new MindControlDevice(),
                new MineralCompressor(),
                new OxygenCapsule(),
                new ProjectileAmmunition(),
                new StarshipLaunchFuel(),
                new UnstablePlasma(),
                new WarpCell()
            };
            return ret;
        }

        private static List<Recipe> GetCraftingComponentRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new Antimatter(),
                new AntimatterHousing(),
                new CarbonNanotubes(),
                new CobaltMirror(),
                new DihydrogenJelly(),
                new HermeticSeal(),
                new MetalPlating(),
                new Microprocessor(),
                new OxygenFilter(),
                new SaltRefractor(),
                new SodiumDiode()
            };
            return ret;
        }

        private static List<Recipe> GetEnhancedGasProductRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new EnrichedCarbon(),
                new NitrogenSalt(),
                new ThermicCondensate()
            };
            return ret;
        }

        private static List<Recipe> GetEnrichedAlloyMetalRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new Geodesite(),
                new Iridesite()
            };
            return ret;
        }

        private static List<Recipe> GetExocraftTechnologyRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new ExocraftAccelerationModule(),
                new ExocraftMiningLaser(),
                new ExocraftMiningLaserSigma(),
                new ExocraftMountedCannon(),
                new ExocraftSignalBoosterSigma(),
                new ExocraftSignalBoosterTau()
            };
            return ret;
        }

        private static List<Recipe> GetExocraftTerminalRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new ColossusGeobay(),
                new ExocraftSummoningStation(),
                new NautilonChamber(),
                new NomadGeobay(),
                new PilgrimGeobay(),
                new RaceForceAmplifier(),
                new RaceInitiator(),
                new RaceObstacle(),
                new RoamerGeobay()
            };
            return ret;
        }

        private static List<Recipe> GetExosuitTechnologyRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new EfficientWaterJets(),
                new HazMatGauntlet(),
                new NeuralStimulator(),
                new OxygenRecycler(),
                new OxygenRerouter(),
                new RocketBoots(),
                new ShieldLattice()
            };
            return ret;
        }

        private static List<Recipe> GetFarmingRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AlbumenPearlOrb(),
                new CopriteFlower(),
                new Echinocactus(),
                new FloraContainment(),
                new Frostwort(),
                new FungalCluster(),
                new GammaWeed(),
                new GravatinoHost(),
                new HydroponicTray(),
                new LargeHydroponicTray(),
                new MorditeRoot(),
                new PottedPlant(),
                new SolarVine(),
                new StandingPlanter(),
                new StarBramble(),
                new VenomUrchin()
            };
            return ret;
        }

        private static List<Recipe> GetFreighterTechnologyRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new FreighterWarpReactorSigma(),
                new FreighterWarpReactorTau(),
                new FreighterWarpReactorTheta()
            };
            return ret;
        }

        private static List<Recipe> GetHighlyRefinedTechnologyRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new FusionIgnitor(),
                new StasisDevice()
            };
            return ret;
        }

        private static List<Recipe> GetManufacturedGasProductRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new CryoPump(),
                new FusionAccelerant(),
                new HotIce(),
                new OrganicCatalyst(),
                new Semiconductor(),
                new Superconductor()
            };
            return ret;
        }

        private static List<Recipe> GetMultitoolTechnologyRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AdvancedMiningLaser(),
                new AmplifiedCartridges(),
                new AnalysisVisor(),
                new BarrelIoniser(),
                new BlazeJavelin(),
                new Boltcaster(),
                new BoltcasterSM(),
                new CombatScope(),
                new GeologyCannon(),
                new MassAccelerator(),
                new PersonalForcefield(),
                new PlasmaLauncher(),
                new PlasmaResonator(),
                new PulseSplitter(),
                new Scanner(),
                new ScatterBlaster(),
                new ShellGreaser(),
                new TerrainManipulator(),
                new WaveformRecycler()
            };
            return ret;
        }

        private static List<Recipe> GetPortableTechnologyRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AtmosphereHarvester(),
                new AutonomousMiningUnit(),
                new BaseComputer(),
                new Beacon(),
                new BlueprintAnalyser(),
                new CommunicationStation(),
                new MarineShelter(),
                new PortableRefiner (),
                new SavePoint(),
                new SignalBooster()
            };
            return ret;
        }

        private static List<Recipe> GetStarshipTechnologyRecipes()
        {
            List<Recipe> ret = new List<Recipe>
            {
                new AblativeArmour(),
                new CadmiumDrive(),
                new ConflictScanner(),
                new CyclotronBallista(),
                new DysonPump(),
                new EconomyScanner(),
                new EfficientThrusters(),
                new EmerilDrive(),
                new FourierDelimiter(),
                new FragmentSupercharger(),
                new IndiumDrive(),
                new InfraKnifeAccelerator(),
                new LargeRocketTubes(),
                new NonlinearOptics(),
                new NonLinearOpticsIKA(),
                new PhaseBeam(),
                new PositronEjector(),
                new RocketLauncher(),
                new TeleportReceiver()
            };
            return ret;
        }

        private static List<Recipe> GetAllRecipes()
        {
            List<Recipe> ret = new List<Recipe>();

            ret.AddRange(GetAccessCardRecipes());
            ret.AddRange(GetAdvancedAgriculturalProductRecipes());
            ret.AddRange(GetAdvancedCraftedProductRecipes());
            ret.AddRange(GetAlloyMetalRecipes());
            ret.AddRange(GetAtlasSeedRecipes());
            ret.AddRange(GetBaseAquaticConstructionRecipes());
            ret.AddRange(GetBaseConcreteComponenetsRecipes());
            ret.AddRange(GetBaseMetalComponentRecipes());
            ret.AddRange(GetBaseWoodComponentRecipes());
            ret.AddRange(GetBaseDecorationRecipes());
            ret.AddRange(GetBaseEquipmentRecipes());
            ret.AddRange(GetBaseStorageRecipes());
            ret.AddRange(GetBaseStructureRecipes());
            ret.AddRange(GetBaseTerminalRecipes());
            ret.AddRange(GetConcentratedDepositRecipes());
            ret.AddRange(GetConsumableRecipes());
            ret.AddRange(GetCraftingComponentRecipes());
            ret.AddRange(GetEnhancedGasProductRecipes());
            ret.AddRange(GetEnrichedAlloyMetalRecipes());
            ret.AddRange(GetExocraftTechnologyRecipes());
            ret.AddRange(GetExocraftTerminalRecipes());
            ret.AddRange(GetExosuitTechnologyRecipes());
            ret.AddRange(GetFarmingRecipes());
            ret.AddRange(GetFreighterTechnologyRecipes());
            ret.AddRange(GetHighlyRefinedTechnologyRecipes());
            ret.AddRange(GetManufacturedGasProductRecipes());
            ret.AddRange(GetMultitoolTechnologyRecipes());
            ret.AddRange(GetPortableTechnologyRecipes());
            ret.AddRange(GetStarshipTechnologyRecipes());

            return ret;
        }
    }
}
