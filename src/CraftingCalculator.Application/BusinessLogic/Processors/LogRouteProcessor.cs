using CraftingCalculator.Application.Common.Utils;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Turns an app route into the form the diagnostic log records it in: the screen, with nothing that identifies a
/// record in it.
/// </summary>
public static class LogRouteProcessor
{
    /// <summary>What a route segment the app does not know is recorded as.</summary>
    public const string UnknownSegment = "{unknown}";

    /// <summary>What a record id in a route is recorded as.</summary>
    public const string IdSegment = "{id}";

    /// <summary>What the id of a record that is not saved yet is recorded as.</summary>
    public const string NewSegment = "new";

    private const string UnsavedId = "0";

    // Every segment the app's own routes are made of. A segment is only ever written to the log if it is in here,
    // so a route that someday carries a name or a search term is recorded as {unknown} rather than leaking it.
    private static readonly HashSet<string> KnownSegments =
    [
        "favorites", "dataset", "import-export", "export", "import", HelpTopics.HelpRoot, "settings", "not-found",
        .. Enum.GetNames<DataType>().Select(name => name.ToLowerInvariant()),
        .. HelpTopics.All.Select(topic => topic.Id)
    ];

    /// <summary>
    /// <paramref name="route"/> as a template: lowercased, without its query string or fragment, a record id as
    /// <see cref="IdSegment"/> (<see cref="NewSegment"/> for an unsaved record), and every segment the app does not
    /// know as <see cref="UnknownSegment"/>. <c>"dataset/Blueprint/42?copyFrom=7"</c> is
    /// <c>"/dataset/blueprint/{id}"</c>; the root route is <c>"/"</c>.
    /// </summary>
    /// <param name="route">An app route relative to the base URI, with or without a leading slash.</param>
    public static string ToTemplate(string? route)
    {
        string normalized = RouteUtil.NormalizeRoute(route);

        if (normalized.Length == 0)
        {
            return "/";
        }

        return "/" + string.Join('/', normalized.Split('/').Select(ToTemplateSegment));
    }

    private static string ToTemplateSegment(string segment)
    {
        if (segment == UnsavedId)
        {
            return NewSegment;
        }

        if (segment.Length > 0 && segment.All(char.IsAsciiDigit))
        {
            return IdSegment;
        }

        return KnownSegments.Contains(segment) ? segment : UnknownSegment;
    }
}
