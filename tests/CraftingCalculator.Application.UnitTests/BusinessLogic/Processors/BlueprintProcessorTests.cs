using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BlueprintProcessorTests
{
    private static BlueprintNode BlueprintNodeOf(long quantity, long crafts) =>
        new(new BlueprintModel { Id = 1, Name = "Bracket" },
            Quantity: quantity, Crafts: crafts, Yield: 2, Surplus: 0, FromSurplus: 0, ProductionTime: TimeSpan.Zero,
            Children: []);

    [TestCase(0, 2, ExpectedResult = 0)]
    [TestCase(-5, 2, ExpectedResult = 0)]
    [TestCase(1, 1, ExpectedResult = 1)]
    [TestCase(4, 1, ExpectedResult = 4)]
    [TestCase(1, 2, ExpectedResult = 1)]
    [TestCase(2, 2, ExpectedResult = 1)]
    [TestCase(3, 2, ExpectedResult = 2)]
    [TestCase(4, 2, ExpectedResult = 2)]
    [TestCase(100, 2, ExpectedResult = 50)]
    [TestCase(10, 3, ExpectedResult = 4)]
    public long CraftsFor_RoundsUpToWholeCrafts(long quantity, long yield) =>
        BlueprintProcessor.CraftsFor(quantity, yield);

    [TestCase(0)]
    [TestCase(-1)]
    public void CraftsFor_YieldBelowOne_Throws(long yield)
    {
        Action act = () => BlueprintProcessor.CraftsFor(4, yield);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void CountsByCraft_FewerCraftsThanItems_IsTrue()
    {
        BlueprintProcessor.CountsByCraft(BlueprintNodeOf(quantity: 2, crafts: 1)).Should().BeTrue();
    }

    [Test]
    public void CountsByCraft_StepCoveredBySurplus_IsTrue()
    {
        // Both Brackets came from an earlier step's surplus, so this step takes no crafts at all.
        BlueprintProcessor.CountsByCraft(BlueprintNodeOf(quantity: 2, crafts: 0)).Should().BeTrue();
    }

    [Test]
    public void CountsByCraft_AsManyCraftsAsItems_IsFalse()
    {
        // One Bracket still takes one craft, so there is no second number to tell the user about.
        BlueprintProcessor.CountsByCraft(BlueprintNodeOf(quantity: 1, crafts: 1)).Should().BeFalse();
    }

    [Test]
    public void CountsByCraft_Component_IsFalse()
    {
        BlueprintNode leaf = new(new ComponentModel { Id = 1, Name = "Screw" },
            Quantity: 3, Crafts: 0, Yield: 0, Surplus: 0, FromSurplus: 0, ProductionTime: TimeSpan.Zero,
            Children: []);

        BlueprintProcessor.CountsByCraft(leaf).Should().BeFalse();
    }

    [Test]
    public void CountsByCraft_AlreadyCrafted_IsFalse()
    {
        // 24 Bronze and no crafts would otherwise read as a step its surplus covered.
        BlueprintNode marked = BlueprintNodeOf(quantity: 24, crafts: 0) with { AlreadyCrafted = true };

        BlueprintProcessor.CountsByCraft(marked).Should().BeFalse();
    }

    private static readonly IReadOnlySet<int> NoStops = new HashSet<int>();

    private static BlueprintModel NewBlueprint(int id, string name, params BlueprintModel[] children)
    {
        BlueprintModel blueprint = new() { Id = id, Name = name };
        foreach (BlueprintModel child in children)
        {
            blueprint.ChildBlueprints.Add(child, 1);
        }

        return blueprint;
    }

    [Test]
    public void NestedBlueprints_RootNestedNowhere_IsLeftOut()
    {
        BlueprintModel bronze = NewBlueprint(1, "Bronze");
        BlueprintModel axe = NewBlueprint(2, "Bronze Axe", bronze);

        BlueprintProcessor.NestedBlueprints([axe], NoStops).Keys.Should().BeEquivalentTo([1]);
    }

    [Test]
    public void NestedBlueprints_RootNestedUnderAnotherRoot_IsIncluded()
    {
        BlueprintModel bronze = NewBlueprint(1, "Bronze");
        BlueprintModel axe = NewBlueprint(2, "Bronze Axe", bronze);

        BlueprintProcessor.NestedBlueprints([axe, bronze], NoStops).Keys.Should().BeEquivalentTo([1]);
    }

    [Test]
    public void NestedBlueprints_StopAt_IsIncludedButNotWalked()
    {
        BlueprintModel rivets = NewBlueprint(1, "Rivet Set");
        BlueprintModel bracket = NewBlueprint(2, "Bracket", rivets);
        BlueprintModel frame = NewBlueprint(3, "Frame", bracket);

        BlueprintProcessor.NestedBlueprints([frame], new HashSet<int> { 2 }).Keys.Should().BeEquivalentTo([2]);
        BlueprintProcessor.NestedBlueprints([frame], NoStops).Keys.Should().BeEquivalentTo([1, 2]);
    }

    [Test]
    public void NestedBlueprints_SharedSubRecipe_IsListedOnceWhicheverCopyReachesIt()
    {
        // Separately loaded entries each carry their own copy of Bronze.
        BlueprintModel axe = NewBlueprint(2, "Bronze Axe", NewBlueprint(1, "Bronze"));
        BlueprintModel nails = NewBlueprint(3, "Bronze Nails", NewBlueprint(1, "Bronze"));

        Dictionary<int, BlueprintModel> nested = BlueprintProcessor.NestedBlueprints([axe, nails], NoStops);

        nested.Should().ContainSingle().Which.Value.Should().BeSameAs(axe.ChildBlueprints.BlueprintList[0].Blueprint);
    }

    [Test]
    public async Task NestedBlueprints_SharedSubRecipes_AreWalkedOncePerBlueprintRatherThanOncePerPath()
    {
        // Forty levels where both blueprints on each level nest both on the next: 2^40 paths, but only 80 blueprints.
        BlueprintModel[] level = [NewBlueprint(1, "A"), NewBlueprint(2, "B")];
        for (int depth = 1; depth < 40; depth++)
        {
            level = [NewBlueprint(depth * 2 + 1, "A", level), NewBlueprint(depth * 2 + 2, "B", level)];
        }

        BlueprintModel top = NewBlueprint(0, "Top", level);
        Dictionary<int, BlueprintModel>? nested = null;

        Func<Task> walk = () => Task.Run(() => nested = BlueprintProcessor.NestedBlueprints([top], NoStops));

        await walk.Should().CompleteWithinAsync(TimeSpan.FromSeconds(10));
        nested.Should().HaveCount(80);
    }
}
