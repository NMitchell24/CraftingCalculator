using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class BlueprintDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private BlueprintDAO _blueprintDAO = null!;
    private ComponentDAO _componentDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _blueprintDAO = new BlueprintDAO(_fixture.DatasetFactory);
        _componentDAO = new ComponentDAO(_fixture.DatasetFactory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task SaveAsync_NewBlueprintWithComponentAndChildBlueprint_PersistsTheFullGraph()
    {
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 1 });
        BlueprintModel plank = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });

        BlueprintModel table = new() { Name = "Table", Value = 10 };
        table.Components.Add(wood, 2);
        table.ChildBlueprints.Add(plank, 4);

        BlueprintModel saved = await _blueprintDAO.SaveAsync(table);
        saved.Id.Should().BeGreaterThan(0);

        BlueprintModel? reloaded = await _blueprintDAO.GetByIdAsync(saved.Id);
        reloaded.Should().NotBeNull();
        reloaded.Components.ComponentList.Should().ContainSingle(componentQuantity => componentQuantity.Component.Name == "Wood" && componentQuantity.Quantity == 2);
        reloaded.ChildBlueprints.BlueprintList.Should().ContainSingle(blueprintQuantity => blueprintQuantity.Blueprint.Name == "Plank" && blueprintQuantity.Quantity == 4);
    }

    /// <summary>
    /// The Craft screen reads its components out of the blueprint graph rather than through
    /// <see cref="ComponentDAO" />, and that graph's entities are detached, so the category has to be
    /// resolved from the graph's own category lookup. Covers a nested child blueprint too, which is the
    /// depth the Components list actually flattens from.
    /// </summary>
    [Test]
    public async Task GetByIdAsync_ComponentsInTheGraph_CarryTheirOwnCategory()
    {
        CategoryDAO categoryDAO = new(_fixture.DatasetFactory);
        CategoryModel ores = await categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        CategoryModel parts = await categoryDAO.SaveAsync(new CategoryModel { Name = "Parts" });

        ComponentModel iron = await _componentDAO.SaveAsync(new ComponentModel { Name = "Iron", Category = ores });
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood" });

        BlueprintModel bracket = new() { Name = "Bracket", Category = parts };
        bracket.Components.Add(iron, 2);
        bracket = await _blueprintDAO.SaveAsync(bracket);

        BlueprintModel frame = new() { Name = "Frame" };
        frame.Components.Add(wood, 1);
        frame.ChildBlueprints.Add(bracket, 3);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(frame);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;

        reloaded.Components.ComponentList.Should()
            .ContainSingle(componentQuantity => componentQuantity.Component.Name == "Wood"
                && componentQuantity.Component.Category == null);

        BlueprintModel child = reloaded.ChildBlueprints.BlueprintList.Single().Blueprint;
        child.Category!.Name.Should().Be("Parts");
        child.Components.ComponentList.Should()
            .ContainSingle(componentQuantity => componentQuantity.Component.Name == "Iron"
                && componentQuantity.Component.Category!.Name == "Ores");
    }

    [Test]
    public async Task SaveAsync_Yield_RoundTripsThroughTheDatabase()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bracket", Yield = 4 });

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloaded.Yield.Should().Be(4);

        reloaded.Yield = 7;
        await _blueprintDAO.SaveAsync(reloaded);

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.Yield.Should().Be(7);
    }

    /// <summary>
    /// Production time is stored as ticks through a value converter, so this covers the whole round trip
    /// rather than just the property: a tenth of a second has to survive the conversion in both
    /// directions, and on a component as well as a blueprint.
    /// </summary>
    [Test]
    public async Task SaveAsync_ProductionTime_RoundTripsThroughTheDatabase()
    {
        ComponentModel potato = await _componentDAO.SaveAsync(
            new ComponentModel { Name = "Potato", ProductionTime = TimeSpan.FromMinutes(5) });

        BlueprintModel soup = new() { Name = "Soup", ProductionTime = TimeSpan.FromSeconds(5.5) };
        soup.Components.Add(potato, 2);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(soup);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloaded.ProductionTime.Should().Be(TimeSpan.FromSeconds(5.5));
        reloaded.Components.ComponentList[0].Component.ProductionTime.Should().Be(TimeSpan.FromMinutes(5));

        reloaded.ProductionTime = TimeSpan.FromHours(2);
        await _blueprintDAO.SaveAsync(reloaded);

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.ProductionTime.Should().Be(TimeSpan.FromHours(2));
    }

    [Test]
    public async Task SaveAsync_WithoutAnExplicitProductionTime_DefaultsToInstant()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.ProductionTime.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public async Task SaveAsync_BlueprintWithoutAnExplicitYield_DefaultsToOne()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.Yield.Should().Be(1);
    }

    [Test]
    public async Task SaveAsync_UpdatingExistingComponentQuantity_UpdatesInPlaceRatherThanDuplicating()
    {
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 1 });
        BlueprintModel table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(table);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        ComponentQuantity existingComponent = reloaded.Components.ComponentList.Single();
        existingComponent.Quantity = 9;

        await _blueprintDAO.SaveAsync(reloaded);

        BlueprintModel reloadedAgain = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().ContainSingle();
        reloadedAgain.Components.ComponentList[0].Quantity.Should().Be(9);
    }

    [Test]
    public async Task SaveAsync_RemovedComponent_DeletesItsRowRatherThanLeavingItOrphaned()
    {
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 1 });
        BlueprintModel table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(table);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloaded.Components.Remove(wood, reloaded.Components.ComponentList[0].Quantity);

        await _blueprintDAO.SaveAsync(reloaded);

        BlueprintModel reloadedAgain = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAsync_RemovesTheBlueprint()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Table" });

        await _blueprintDAO.DeleteAsync(saved.Id);

        (await _blueprintDAO.GetByIdAsync(saved.Id)).Should().BeNull();
    }

    /// <summary>
    /// A database written by a build whose editor let a blueprint nest one of its own ancestors has a
    /// loop in BlueprintChildren. Loading it used to throw, and because nothing in the UI catches that,
    /// every screen reading a blueprint died with it and the app could not be recovered without wiping
    /// the data. The back edge is dropped instead.
    /// </summary>
    [Test]
    public async Task GetAllAsync_ABlueprintNestingItsOwnAncestor_LoadsWithTheLoopBrokenRatherThanThrowing()
    {
        BlueprintModel bracket = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bracket" });

        BlueprintModel frame = new() { Name = "Frame" };
        frame.ChildBlueprints.Add(bracket, 2);
        frame = await _blueprintDAO.SaveAsync(frame);

        BlueprintModel cyclic = (await _blueprintDAO.GetByIdAsync(bracket.Id))!;
        cyclic.ChildBlueprints.Add(frame, 1);
        await _blueprintDAO.SaveAsync(cyclic);

        List<BlueprintModel> all = await _blueprintDAO.GetAllAsync();

        BlueprintModel loadedFrame = all.Single(blueprint => blueprint.Name == "Frame");
        BlueprintModel nestedBracket = loadedFrame.ChildBlueprints.BlueprintList.Single().Blueprint;
        nestedBracket.Name.Should().Be("Bracket");
        nestedBracket.ChildBlueprints.BlueprintList.Should().BeEmpty();

        // Read from the other end of the loop the nesting is the one that survives, because Frame is
        // then the ancestor being skipped rather than the root.
        BlueprintModel loadedBracket = all.Single(blueprint => blueprint.Name == "Bracket");
        loadedBracket.ChildBlueprints.BlueprintList.Single().Blueprint.Name.Should().Be("Frame");
    }

    /// <summary>
    /// The ancestor path is per-branch, not a visited set for the whole walk: one blueprint nested by
    /// two siblings is a diamond, and both copies have to hydrate in full.
    /// </summary>
    [Test]
    public async Task GetByIdAsync_OneBlueprintNestedByTwoSiblings_HydratesBothCopies()
    {
        ComponentModel iron = await _componentDAO.SaveAsync(new ComponentModel { Name = "Iron", Cost = 1 });

        BlueprintModel screw = new() { Name = "Screw" };
        screw.Components.Add(iron, 1);
        screw = await _blueprintDAO.SaveAsync(screw);

        BlueprintModel bracket = new() { Name = "Bracket" };
        bracket.ChildBlueprints.Add(screw, 4);
        bracket = await _blueprintDAO.SaveAsync(bracket);

        BlueprintModel plate = new() { Name = "Plate" };
        plate.ChildBlueprints.Add(screw, 2);
        plate = await _blueprintDAO.SaveAsync(plate);

        BlueprintModel hull = new() { Name = "Hull" };
        hull.ChildBlueprints.Add(bracket, 1);
        hull.ChildBlueprints.Add(plate, 1);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(hull);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;

        reloaded.ChildBlueprints.BlueprintList.Should().HaveCount(2);
        foreach (BlueprintQuantity branch in reloaded.ChildBlueprints.BlueprintList)
        {
            BlueprintModel nestedScrew = branch.Blueprint.ChildBlueprints.BlueprintList.Single().Blueprint;
            nestedScrew.Name.Should().Be("Screw");
            nestedScrew.Components.ComponentList.Should().ContainSingle();
        }
    }
}
