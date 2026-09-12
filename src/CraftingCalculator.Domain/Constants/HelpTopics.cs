using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Domain.Constants;

/// <summary>
/// Every page of the built-in help, in the order the contents list shows them. This catalog is the
/// contract between three things that have to agree: the Markdown files under <c>docs/help</c>, the
/// in-app routes the help button maps from, and the contents list. Adding a page means adding an entry
/// here and the matching <c>docs/help/&lt;id&gt;.md</c>.
/// </summary>
public static class HelpTopics
{
    /// <summary>The page the help button opens when the current route matches no other topic.</summary>
    public const string DefaultTopicId = "welcome";

    /// <summary>The route segment the in-app help pages live under.</summary>
    public const string HelpRoot = "help";

    public static IReadOnlyList<HelpTopic> All { get; } =
    [
        new("welcome", "Welcome",
            "What this app does, and the three words you need to know.",
            [HelpRoot]),

        new("getting-started", "Getting Started",
            "Go from an empty database to your first finished calculation.",
            []),

        new("actions-bar", "Actions and Navigation",
            "How the navigation and actions bars adapt across mobile and desktop layouts.",
            []),

        new("craft-screen", "The Craft Screen",
            "Build a batch, read the totals, and dig into the breakdown.",
            [""]),

        new("categories", "Categories",
            "Labels that keep a big dataset searchable.",
            ["dataset/category"]),

        new("components", "Components",
            "The raw stuff at the bottom of every recipe tree.",
            ["dataset/component"]),

        new("blueprints", "Blueprints",
            "Recipes: what they cost, what they are worth, and how they nest.",
            ["dataset/blueprint"]),

        new("managing-datasets", "Managing Datasets",
            "Keep a separate set of records per game, and switch between them in a tap.",
            []),

        new("dataset", "The Dataset Screen",
            "Browse, search, duplicate and delete your records.",
            ["dataset"]),

        new("favorites", "Favorites",
            "Save a batch you will build again.",
            ["favorites"]),

        new("calculations", "How the Math Works",
            "Yield, surplus, crafting steps, cost, value, profit and time - shown with worked examples.",
            []),

        new("settings", "Settings",
            "Theme, version, and where your data lives.",
            ["settings"]),

        new("tips", "Tips and Troubleshooting",
            "Modeling tricks for real games, and what to do when a number looks wrong.",
            [])
    ];
}
