namespace CraftingCalculator.Application.Common.Utils;

public static class RouteUtil
{
    /// <summary>
    /// <paramref name="route"/> without its query string, fragment, or leading and trailing slashes, lowercased;
    /// empty for the root route.
    /// </summary>
    /// <param name="route">An app route, with or without a leading slash, query string or fragment.</param>
    public static string NormalizeRoute(string? route)
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
