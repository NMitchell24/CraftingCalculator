using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// Serves the built-in help pages. The content ships inside the app, so every page is available
/// offline.
/// </summary>
public interface IHelpService
{
    /// <summary>
    /// The help page with this id, rendered and ready to display, or null when no page has that id.
    /// </summary>
    Task<HelpArticle?> GetArticleAsync(string? topicId);
}
