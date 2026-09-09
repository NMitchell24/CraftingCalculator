using System.Net;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Turns the help Markdown into what the app renders, and decides which help page belongs to a given
/// app route. The Markdown is authored once under <c>docs/help</c> and shared with the project site and
/// the wiki, so everything specific to rendering it inside the app happens here.
/// </summary>
public static class HelpProcessor
{
    /// <summary>The file extension help pages link to each other by.</summary>
    private const string MarkdownExtension = ".md";

    /// <summary>The path help pages reference their icons by, relative to the page itself.</summary>
    private const string AssetPrefix = "assets/";

    private const string IconExtension = ".svg";

    // The fill the icon files carry. It is a mid grey so the same file is legible on both of GitHub's
    // themes, where it renders as an <img> and has no text colour to inherit; inlining swaps it for
    // currentColor, which is what makes the icon follow the app's own light/dark palette.
    private const string IconFileFill = "fill=\"#888888\"";

    // UseYamlFrontMatter is what keeps the Jekyll front matter (title/nav_order, which the project site
    // and the wiki sync read) out of the rendered body. The rest is the subset of Markdown the help
    // content is written in - no raw HTML extension, since the same files have to render as plain
    // Markdown on GitHub.
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseYamlFrontMatter()
        .UsePipeTables()
        .UseAutoIdentifiers()
        .UseEmphasisExtras()
        .Build();

    /// <summary>
    /// The help page for <paramref name="route"/> - the page whose longest matching route prefix covers
    /// it - or the Welcome page when nothing matches.
    /// </summary>
    /// <param name="route">
    /// An app route, with or without a leading slash and with or without a query string.
    /// </param>
    public static HelpTopic ResolveTopic(string? route)
    {
        string normalized = NormalizeRoute(route);

        HelpTopic? best = null;
        int bestLength = -1;

        foreach (HelpTopic topic in HelpTopics.All)
        {
            foreach (string prefix in topic.RoutePrefixes)
            {
                if (prefix.Length > bestLength && CoversRoute(prefix, normalized))
                {
                    best = topic;
                    bestLength = prefix.Length;
                }
            }
        }

        return best ?? Find(HelpTopics.DefaultTopicId)!;
    }

    /// <summary>The topic with this id, or null when no such page exists.</summary>
    public static HelpTopic? Find(string? topicId) =>
        HelpTopics.All.FirstOrDefault(topic =>
            string.Equals(topic.Id, topicId, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Renders help Markdown to an HTML fragment: cross-page <c>.md</c> links become the in-app help
    /// routes they correspond to, and each icon image becomes the icon's own markup inline.
    /// </summary>
    /// <param name="markdown">The page's Markdown, front matter included.</param>
    /// <param name="readIcon">
    /// Supplies an icon file's contents by file name, or null when there is no such icon - in which case
    /// the image falls back to its alt text.
    /// </param>
    public static string Render(string markdown, Func<string, string?> readIcon)
    {
        MarkdownDocument document = Markdown.Parse(markdown, Pipeline);

        // Materialised before anything is replaced: substituting an icon detaches its node from the
        // tree, which would cut the walk short partway through the document.
        List<LinkInline> links = [.. document.Descendants<LinkInline>()];

        foreach (LinkInline link in links)
        {
            if (link.IsImage)
            {
                ReplaceIcon(link, readIcon);
                continue;
            }

            link.Url = RewriteLink(link.Url);
        }

        return document.ToHtml(Pipeline);
    }

    // Help pages link to each other the way GitHub expects - "blueprints.md", which the repo browser,
    // the wiki and jekyll-relative-links all resolve on their own. Only the app has to translate.
    private static string? RewriteLink(string? url)
    {
        if (string.IsNullOrEmpty(url) || url.Contains("://") || url.StartsWith('/'))
        {
            return url;
        }

        int anchorStart = url.IndexOf('#');
        string path = anchorStart < 0 ? url : url[..anchorStart];
        string anchor = anchorStart < 0 ? "" : url[anchorStart..];

        if (!path.EndsWith(MarkdownExtension, StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        return $"/{HelpTopics.HelpRoot}/{path[..^MarkdownExtension.Length]}{anchor}";
    }

    /// <summary>
    /// Swaps an <c>assets/&lt;name&gt;.svg</c> image for that icon's own markup, so the glyph is part of
    /// the document rather than a file the WebView has to fetch, and takes its colour from the text
    /// around it. Anything else - including an icon with no file - becomes the image's alt text.
    /// </summary>
    private static void ReplaceIcon(LinkInline image, Func<string, string?> readIcon)
    {
        // The alt text, which is both the icon's accessible name and what it degrades to.
        string label = image.FirstChild is LiteralInline literal ? literal.Content.ToString() : "";

        if (image.Url is not { } url
            || !url.StartsWith(AssetPrefix, StringComparison.Ordinal)
            || !url.EndsWith(IconExtension, StringComparison.OrdinalIgnoreCase)
            || readIcon(url[AssetPrefix.Length..]) is not { } svg)
        {
            Replace(image, new LiteralInline(label));
            return;
        }

        string attributes = $"fill=\"currentColor\" class=\"help-icon\" role=\"img\" aria-label=\"{WebUtility.HtmlEncode(label)}\"";

        Replace(image, new HtmlInline(svg.Trim().Replace(IconFileFill, attributes)));
    }

    // copyChildren defaults to true, which re-parents the image's alt text alongside the replacement -
    // rendering the label twice, once from the replacement and once from the salvaged child.
    private static void Replace(LinkInline image, Inline replacement) =>
        image.ReplaceBy(replacement, copyChildren: false);

    /// <summary>
    /// Whether <paramref name="prefix"/> claims <paramref name="route"/>. The comparison is by whole
    /// segment, so "dataset" covers "dataset/blueprint" but "data" covers neither.
    /// </summary>
    private static bool CoversRoute(string prefix, string route) => prefix.Length == 0
        ? route.Length == 0
        : route.Equals(prefix, StringComparison.Ordinal) || route.StartsWith($"{prefix}/", StringComparison.Ordinal);

    private static string NormalizeRoute(string? route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return "";
        }

        int queryStart = route.IndexOfAny(['?', '#']);
        string path = queryStart < 0 ? route : route[..queryStart];

        return path.Trim('/').ToLowerInvariant();
    }
}
