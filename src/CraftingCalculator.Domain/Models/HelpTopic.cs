namespace CraftingCalculator.Domain.Models;

/// <summary>
/// One page of the built-in help. <see cref="Id"/> is the page's slug: it is the file name of the
/// Markdown the page is written in, the last segment of its in-app route, and the link target other
/// help pages use to reach it.
/// </summary>
/// <param name="Id">The slug, kebab-case.</param>
/// <param name="Title">The page's heading, as the contents list and the app bar show it.</param>
/// <param name="Summary">One line describing the page, shown beneath its title in the contents list.</param>
/// <param name="RoutePrefixes">
/// The app routes this page is the help for, each without its leading slash. The help button opens the
/// page whose longest prefix matches the route the user is on, so a page can claim a whole branch
/// ("dataset/blueprint") without listing every route beneath it. Empty on a page reached only from the
/// contents list.
/// </param>
public sealed record HelpTopic(
    string Id,
    string Title,
    string Summary,
    IReadOnlyList<string> RoutePrefixes)
{
    /// <summary>The Markdown file this page is written in, as embedded in the Application assembly.</summary>
    public string FileName => $"{Id}.md";
}
