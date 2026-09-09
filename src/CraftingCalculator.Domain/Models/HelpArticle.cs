namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A help page ready to render: the topic it came from and its Markdown converted to an HTML fragment.
/// </summary>
/// <param name="Topic">The topic this article is the content of.</param>
/// <param name="Html">
/// The body as an HTML fragment - no document, head or body element. Built from content that ships
/// inside the app, so it is safe to render as raw markup.
/// </param>
public sealed record HelpArticle(HelpTopic Topic, string Html);
