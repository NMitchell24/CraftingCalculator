using System.Text.RegularExpressions;
using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

/// <summary>
/// Runs against the real embedded help content rather than a stub, which is the point: these are the
/// tests that fail when a page is added to <see cref="HelpTopics" /> without its Markdown, when the
/// Markdown stops being embedded, or when a cross-page link is misspelled.
/// </summary>
[TestFixture]
public class HelpServiceTests
{
    /// <summary>Matches the in-app links <see cref="HelpProcessor.Render" /> produces.</summary>
    private static readonly Regex HelpLink = new("href=\"/help/([^\"#]+)", RegexOptions.Compiled);

    private static IEnumerable<HelpTopic> Topics => HelpTopics.All;

    private HelpService _service = null!;

    [SetUp]
    public void SetUp() => _service = new HelpService();

    [TestCaseSource(nameof(Topics))]
    public async Task GetArticleAsync_EveryTopicInTheCatalog_HasContent(HelpTopic topic)
    {
        HelpArticle? article = await _service.GetArticleAsync(topic.Id);

        article.Should().NotBeNull($"{topic.FileName} should exist under docs/help and be embedded");
        article!.Topic.Should().Be(topic);
        article.Html.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// The rendered H1 is the page's own heading, and the router focuses it on navigation
    /// (Components/Routes.razor).
    /// </summary>
    [TestCaseSource(nameof(Topics))]
    public async Task GetArticleAsync_EveryTopic_OpensWithAHeading(HelpTopic topic)
    {
        HelpArticle? article = await _service.GetArticleAsync(topic.Id);

        article!.Html.Should().Contain("<h1");
    }

    [TestCaseSource(nameof(Topics))]
    public async Task GetArticleAsync_EveryTopic_LinksOnlyToTopicsThatExist(HelpTopic topic)
    {
        HelpArticle? article = await _service.GetArticleAsync(topic.Id);

        foreach (Match match in HelpLink.Matches(article!.Html))
        {
            string linked = match.Groups[1].Value;

            HelpProcessor.Find(linked).Should()
                .NotBeNull($"{topic.FileName} links to '{linked}.md', which is not a help topic");
        }
    }

    [TestCase("no-such-page")]
    [TestCase("")]
    [TestCase(null)]
    public async Task GetArticleAsync_AnIdThatIsNotATopic_IsNull(string? topicId)
    {
        (await _service.GetArticleAsync(topicId)).Should().BeNull();
    }

    [Test]
    public void Catalog_TopicIds_AreUnique()
    {
        HelpTopics.All.Select(topic => topic.Id).Should().OnlyHaveUniqueItems();
    }

    /// <summary>
    /// Two topics claiming one route would make the help button's answer depend on catalog order rather
    /// than on anything a reader could predict.
    /// </summary>
    [Test]
    public void Catalog_RoutePrefixes_AreClaimedByOneTopicEach()
    {
        HelpTopics.All.SelectMany(topic => topic.RoutePrefixes).Should().OnlyHaveUniqueItems();
    }

    [Test]
    public void Catalog_ContainsTheDefaultTopic()
    {
        HelpProcessor.Find(HelpTopics.DefaultTopicId).Should().NotBeNull();
    }

    /// <summary>
    /// Every image in the help content is an icon from docs/help/assets, and every one of those is
    /// inlined at render time. A surviving img means the page names a file that is not there - which
    /// renders as the alt text in the app and as a broken image on GitHub and the wiki.
    /// </summary>
    [TestCaseSource(nameof(Topics))]
    public async Task GetArticleAsync_EveryTopic_ReferencesOnlyIconsThatExist(HelpTopic topic)
    {
        HelpArticle? article = await _service.GetArticleAsync(topic.Id);

        article!.Html.Should().NotContain("<img",
            $"{topic.FileName} should only use images from docs/help/assets, and every one should exist");
    }
}
