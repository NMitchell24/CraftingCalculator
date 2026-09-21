using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class LogRouteProcessorTests
{
    /// <summary>Every @page route in the UI project, as the NavigationManager hands them over.</summary>
    [TestCase("favorites", ExpectedResult = "/favorites")]
    [TestCase("dataset", ExpectedResult = "/dataset")]
    [TestCase("dataset/Blueprint", ExpectedResult = "/dataset/blueprint")]
    [TestCase("dataset/Component", ExpectedResult = "/dataset/component")]
    [TestCase("dataset/Category", ExpectedResult = "/dataset/category")]
    [TestCase("dataset/import-export", ExpectedResult = "/dataset/import-export")]
    [TestCase("dataset/import-export/export", ExpectedResult = "/dataset/import-export/export")]
    [TestCase("dataset/import-export/import", ExpectedResult = "/dataset/import-export/import")]
    [TestCase("help", ExpectedResult = "/help")]
    [TestCase("help/calculations", ExpectedResult = "/help/calculations")]
    [TestCase("settings", ExpectedResult = "/settings")]
    [TestCase("not-found", ExpectedResult = "/not-found")]
    public string ToTemplate_AnAppRoute_KeepsEverySegment(string route) => LogRouteProcessor.ToTemplate(route);

    [TestCase("dataset/Blueprint/42", ExpectedResult = "/dataset/blueprint/{id}")]
    [TestCase("/dataset/Category/7", ExpectedResult = "/dataset/category/{id}")]
    [TestCase("dataset/Component/0", ExpectedResult = "/dataset/component/new")]
    public string ToTemplate_ARecordId_IsReplaced(string route) => LogRouteProcessor.ToTemplate(route);

    // The query carries the id of the record being copied, which the template must not.
    [TestCase("dataset/Blueprint/0?copyFrom=7", ExpectedResult = "/dataset/blueprint/new")]
    [TestCase("settings#appearance", ExpectedResult = "/settings")]
    public string ToTemplate_AQueryOrFragment_IsDropped(string route) => LogRouteProcessor.ToTemplate(route);

    [TestCase(null)]
    [TestCase("")]
    [TestCase("/")]
    public void ToTemplate_TheRootRoute_IsASlash(string? route)
    {
        LogRouteProcessor.ToTemplate(route).Should().Be("/");
    }

    /// <summary>
    /// The allow-list is what keeps user content out of the log: a segment it does not name is never written,
    /// whatever it holds.
    /// </summary>
    [TestCase("dataset/Bronze Axe", ExpectedResult = "/dataset/{unknown}")]
    [TestCase("favorites/Longship Run/3", ExpectedResult = "/favorites/{unknown}/{id}")]
    [TestCase("datasets", ExpectedResult = "/{unknown}")]
    [TestCase("dataset/Blueprint/-1", ExpectedResult = "/dataset/blueprint/{unknown}")]
    public string ToTemplate_ASegmentTheAppDoesNotKnow_IsUnknown(string route) => LogRouteProcessor.ToTemplate(route);

    /// <summary>
    /// Trips when a screen is added with a help route but not added to the allow-list, which would otherwise log
    /// that screen as {unknown} forever without anyone noticing.
    /// </summary>
    [Test]
    public void ToTemplate_EveryHelpRoutePrefix_MapsWithoutUnknownSegments()
    {
        IEnumerable<string> prefixes = HelpTopics.All.SelectMany(topic => topic.RoutePrefixes);

        foreach (string prefix in prefixes)
        {
            LogRouteProcessor.ToTemplate(prefix).Should().NotContain(LogRouteProcessor.UnknownSegment, prefix);
        }
    }

    [Test]
    public void ToTemplate_EveryHelpTopic_MapsWithoutUnknownSegments()
    {
        foreach (HelpTopic topic in HelpTopics.All)
        {
            LogRouteProcessor.ToTemplate($"{HelpTopics.HelpRoot}/{topic.Id}")
                .Should().Be($"/{HelpTopics.HelpRoot}/{topic.Id}");
        }
    }
}
