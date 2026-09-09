using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class RecordFilterProcessorTests
{
    private static readonly CategoryModel Metals = new() { Id = 1, Name = "Metals" };
    private static readonly CategoryModel Woods = new() { Id = 2, Name = "Woods" };

    private static ComponentModel Component(string name, CategoryModel? category = null) =>
        new() { Id = name.GetHashCode(), Name = name, Category = category };

    private static RecordFilter Filter(string search = "", params int[] categoryIds) =>
        new(search, new HashSet<int>(categoryIds));

    private static List<IBaseDataRecord> Records() =>
    [
        Component("Iron Ingot", Metals),
        Component("Copper Ingot", Metals),
        Component("Oak Plank", Woods),
        Component("Mystery Dust")
    ];

    [Test]
    public void Apply_EmptyFilter_ReturnsEverything()
    {
        RecordFilterProcessor.Apply(Records(), RecordFilter.Empty).Should().HaveCount(4);
    }

    [TestCase("ingot")]
    [TestCase("INGOT")]
    [TestCase("Ingot")]
    public void Apply_Search_IgnoresCase(string search)
    {
        RecordFilterProcessor.Apply(Records(), Filter(search))
            .Select(record => record.Name)
            .Should().Equal("Iron Ingot", "Copper Ingot");
    }

    [Test]
    public void Apply_Search_MatchesASubstringAnywhereInTheName()
    {
        RecordFilterProcessor.Apply(Records(), Filter("k Pla"))
            .Select(record => record.Name)
            .Should().Equal("Oak Plank");
    }

    [Test]
    public void Apply_OneCategoryId_ReturnsOnlyThatCategory()
    {
        RecordFilterProcessor.Apply(Records(), Filter("", Woods.Id))
            .Select(record => record.Name)
            .Should().Equal("Oak Plank");
    }

    [Test]
    public void Apply_SeveralCategoryIds_AreAnOr()
    {
        RecordFilterProcessor.Apply(Records(), Filter("", Metals.Id, Woods.Id))
            .Select(record => record.Name)
            .Should().Equal("Iron Ingot", "Copper Ingot", "Oak Plank");
    }

    [Test]
    public void Apply_UncategorizedId_ReturnsOnlyRecordsWithNoCategory()
    {
        RecordFilterProcessor.Apply(Records(), Filter("", RecordFilter.UncategorizedId))
            .Select(record => record.Name)
            .Should().Equal("Mystery Dust");
    }

    [Test]
    public void Apply_UncategorizedIdWithARealCategory_ReturnsBothSets()
    {
        RecordFilterProcessor.Apply(Records(), Filter("", RecordFilter.UncategorizedId, Woods.Id))
            .Select(record => record.Name)
            .Should().Equal("Oak Plank", "Mystery Dust");
    }

    /// <summary>
    /// The Categories list is the case: a category is not filed under a category, so a category filter
    /// has nothing to say about it. The search bar hides the filter there anyway.
    /// </summary>
    [Test]
    public void Apply_ARecordThatIsNotCategorized_IsNeverExcludedByACategoryFilter()
    {
        List<IBaseDataRecord> categories = [Metals, Woods];

        RecordFilterProcessor.Apply(categories, Filter("", Woods.Id)).Should().HaveCount(2);
    }

    [Test]
    public void Apply_SearchAndCategory_Compose()
    {
        RecordFilterProcessor.Apply(Records(), Filter("ingot", Metals.Id, Woods.Id))
            .Select(record => record.Name)
            .Should().Equal("Iron Ingot", "Copper Ingot");
    }

    [Test]
    public void CategoriesInUse_ReturnsEachCategoryOnceOrderedByName()
    {
        RecordFilterProcessor.CategoriesInUse(Records())
            .Select(category => category.Name)
            .Should().Equal("Metals", "Woods");
    }

    [Test]
    public void CategoriesInUse_ARecordThatIsNotCategorized_ContributesNothing()
    {
        List<IBaseDataRecord> categories = [Metals, Woods];

        RecordFilterProcessor.CategoriesInUse(categories).Should().BeEmpty();
    }

    [Test]
    public void HasUncategorized_IsFalse_WhenEveryRecordIsFiled()
    {
        List<IBaseDataRecord> filed = [Component("Iron Ingot", Metals), Component("Oak Plank", Woods)];

        RecordFilterProcessor.HasUncategorized(filed).Should().BeFalse();
        RecordFilterProcessor.HasUncategorized(Records()).Should().BeTrue();
    }

    [Test]
    public void Prune_KeepsIdsThatAreStillInUse()
    {
        RecordFilter filter = Filter("iron", Metals.Id);

        RecordFilterProcessor.Prune(filter, Records()).Should().BeSameAs(filter);
    }

    /// <summary>
    /// The one case the search bar exists to survive: recategorising the last record in a category, or
    /// deleting the category, leaves an id that would silently filter everything out.
    /// </summary>
    [Test]
    public void Prune_DropsAnIdNoRecordUsesAnyMore()
    {
        RecordFilter filter = Filter("iron", Metals.Id, Woods.Id);
        List<IBaseDataRecord> remaining = [Component("Iron Ingot", Metals), Component("Mystery Dust")];

        RecordFilter pruned = RecordFilterProcessor.Prune(filter, remaining);

        pruned.CategoryIds.Should().ContainSingle().Which.Should().Be(Metals.Id);
        pruned.Search.Should().Be("iron");
    }

    [Test]
    public void Prune_DropsUncategorized_WhenEveryRecordIsFiled()
    {
        RecordFilter filter = Filter("", RecordFilter.UncategorizedId);
        List<IBaseDataRecord> filed = [Component("Iron Ingot", Metals)];

        RecordFilterProcessor.Prune(filter, filed).CategoryIds.Should().BeEmpty();
    }

    [Test]
    public void Prune_AnEmptySelection_IsLeftAlone()
    {
        RecordFilterProcessor.Prune(RecordFilter.Empty, []).Should().BeSameAs(RecordFilter.Empty);
    }
}
