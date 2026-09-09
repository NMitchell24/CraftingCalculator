using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Constants;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class HelpProcessorTests
{
    /// <summary>Stands in for one of the files under docs/help/assets.</summary>
    private const string StubIcon =
        "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" width=\"20\" height=\"20\" "
        + "fill=\"#888888\"><path d=\"M0 0h24v24H0z\"/></svg>";

    /// <summary>Renders with an icon resolver that knows one icon, "delete.svg".</summary>
    private static string Render(string markdown) =>
        HelpProcessor.Render(markdown, file => file == "delete.svg" ? StubIcon : null);

    [TestCase("", ExpectedResult = "craft-screen")]
    [TestCase("/", ExpectedResult = "craft-screen")]
    // Null is the root route, not an unknown one: the Craft screen is what "/" renders.
    [TestCase(null, ExpectedResult = "craft-screen")]
    [TestCase("favorites", ExpectedResult = "favorites")]
    [TestCase("/settings", ExpectedResult = "settings")]
    [TestCase("dataset", ExpectedResult = "dataset")]
    public string ResolveTopic_APageWithItsOwnHelp_IsThatPagesTopic(string? route) =>
        HelpProcessor.ResolveTopic(route).Id;

    /// <summary>
    /// The dataset routes are the case the longest-prefix rule exists for: "dataset" and
    /// "dataset/blueprint" both match a blueprint route, and the more specific one has to win.
    /// </summary>
    [TestCase("dataset/blueprint", ExpectedResult = "blueprints")]
    [TestCase("dataset/Blueprint/3", ExpectedResult = "blueprints")]
    [TestCase("/dataset/Component/0", ExpectedResult = "components")]
    [TestCase("dataset/Category", ExpectedResult = "categories")]
    public string ResolveTopic_ADatasetRoute_PrefersTheMostSpecificTopic(string route) =>
        HelpProcessor.ResolveTopic(route).Id;

    [TestCase("dataset/Blueprint/0?copyFrom=7", ExpectedResult = "blueprints")]
    [TestCase("settings#appearance", ExpectedResult = "settings")]
    public string ResolveTopic_ARouteWithAQueryOrFragment_IgnoresIt(string route) =>
        HelpProcessor.ResolveTopic(route).Id;

    /// <summary>
    /// A prefix claims whole segments only. "dataset" must not claim a hypothetical "datasets" route,
    /// which a plain StartsWith would.
    /// </summary>
    [TestCase("datasets")]
    [TestCase("favorites-archive")]
    [TestCase("some/unknown/page")]
    public void ResolveTopic_ARouteNoTopicClaims_IsTheDefaultTopic(string route)
    {
        HelpProcessor.ResolveTopic(route).Id.Should().Be(HelpTopics.DefaultTopicId);
    }

    [Test]
    public void Find_AnUnknownId_IsNull()
    {
        HelpProcessor.Find("no-such-page").Should().BeNull();
    }

    [Test]
    public void Find_IgnoresCase()
    {
        HelpProcessor.Find("CRAFT-SCREEN")?.Id.Should().Be("craft-screen");
    }

    /// <summary>
    /// The front matter is what the project site and the wiki sync read; it must never reach the app's
    /// rendered output.
    /// </summary>
    [Test]
    public void Render_FrontMatter_IsNotInTheOutput()
    {
        string html = Render("---\ntitle: Welcome\nnav_order: 1\n---\n\n# Hello\n");

        html.Should().NotContain("nav_order").And.Contain("<h1");
    }

    [Test]
    public void Render_ALinkToAnotherHelpPage_BecomesItsInAppRoute()
    {
        Render("[Blueprints](blueprints.md)")
            .Should().Contain("href=\"/help/blueprints\"");
    }

    [Test]
    public void Render_ALinkToAnAnchorInAnotherHelpPage_KeepsTheAnchor()
    {
        Render("[Surplus](calculations.md#surplus)")
            .Should().Contain("href=\"/help/calculations#surplus\"");
    }

    [TestCase("[Same page](#surplus)", "href=\"#surplus\"")]
    [TestCase("[Absolute](/dataset)", "href=\"/dataset\"")]
    [TestCase("[External](https://example.com/x.md)", "href=\"https://example.com/x.md\"")]
    public void Render_ALinkThatIsNotToAHelpPage_IsLeftAlone(string markdown, string expected)
    {
        Render(markdown).Should().Contain(expected);
    }

    /// <summary>Tables carry a lot of the help content, so the extension has to stay enabled.</summary>
    [Test]
    public void Render_APipeTable_BecomesATable()
    {
        Render("| A | B |\n|---|---|\n| 1 | 2 |\n").Should().Contain("<table");
    }

    /// <summary>
    /// Cross-page anchors are written against these ids, and nothing else in the app would notice if
    /// the extension were dropped from the pipeline.
    /// </summary>
    [Test]
    public void Render_AHeading_GetsAnIdToLinkTo()
    {
        Render("## Surplus").Should().Contain("id=\"surplus\"");
    }

    /// <summary>
    /// The icon is inlined rather than left as an image: the WebView would have to fetch a file that is
    /// not served, and an img cannot take its colour from the text around it.
    /// </summary>
    [Test]
    public void Render_AnIconImage_BecomesTheIconsOwnMarkup()
    {
        string html = Render("![Delete](assets/delete.svg)");

        html.Should().Contain("<svg").And.NotContain("<img");
    }

    /// <summary>
    /// The grey in the file is for GitHub, where the icon renders as an image with no text colour to
    /// inherit. In the app it has to follow the palette instead.
    /// </summary>
    [Test]
    public void Render_AnIconImage_TakesItsColourFromTheSurroundingText()
    {
        string html = Render("![Delete](assets/delete.svg)");

        html.Should().Contain("fill=\"currentColor\"").And.NotContain("#888888");
    }

    [Test]
    public void Render_AnIconImage_CarriesItsAltTextAsTheAccessibleName()
    {
        Render("![Delete](assets/delete.svg)").Should().Contain("aria-label=\"Delete\"");
    }

    /// <summary>
    /// The alt text becomes the accessible name and nothing else. Markdig's replacement salvages an
    /// inline's children by default, which would print the label beside the icon as well.
    /// </summary>
    [Test]
    public void Render_AnIconImage_DoesNotAlsoPrintItsAltText()
    {
        string html = Render("Tap ![Delete](assets/delete.svg) now.");

        html.Should().Contain("aria-label=\"Delete\"").And.NotContain(">Delete");
    }

    [Test]
    public void Render_AnIconWithNoFile_FallsBackToItsAltText()
    {
        string html = Render("Tap ![Save](assets/save.svg) to keep it.");

        html.Should().Contain("Tap Save to keep it.").And.NotContain("<img");
    }

    /// <summary>
    /// Help content has no images other than icons, so anything else is a mistake that should degrade to
    /// words rather than to a broken image.
    /// </summary>
    [Test]
    public void Render_AnImageThatIsNotAnIcon_FallsBackToItsAltText()
    {
        string html = Render("![A screenshot](screenshot.png)");

        html.Should().Contain("A screenshot").And.NotContain("<img");
    }
}
