using CraftingCalculator.Domain.Enums;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The prompts the export and import selection panels run when a tap would change more records than the one
/// tapped. <c>subject</c> is how the prompt names what was tapped: a quoted record name, or a phrase such as
/// "your categories" for a whole panel.
/// </summary>
public static class TransferPrompts
{
    /// <summary>
    /// Asks before a deselect also deselects the <paramref name="cascaded"/> records that depend on
    /// <paramref name="subject"/>. Returns true only if the user chose to continue.
    /// </summary>
    public static async Task<bool> ConfirmDeselectAsync(
        IDialogService dialogs, string subject, IReadOnlyDictionary<RecordKind, int> cascaded)
    {
        bool? confirmed = await dialogs.ShowMessageBoxAsync(
            "Deselect these too?", DeselectMessage(subject, cascaded), yesText: "Continue", cancelText: "Cancel");

        return confirmed == true;
    }

    /// <summary>
    /// Offers to select the records linked to <paramref name="subject"/>, and what those need, which comes to
    /// the <paramref name="cascaded"/> records. Returns true only if the user said yes.
    /// </summary>
    public static async Task<bool> ConfirmSelectUsersAsync(
        IDialogService dialogs, string subject, IReadOnlyDictionary<RecordKind, int> cascaded)
    {
        bool? confirmed = await dialogs.ShowMessageBoxAsync(
            "Select linked items?",
            $"Want everything linked to {subject} too? That selects {Describe(cascaded)}, counting whatever "
            + "those need to be crafted.",
            yesText: "Yes", cancelText: "No");

        return confirmed == true;
    }

    private static string DeselectMessage(string subject, IReadOnlyDictionary<RecordKind, int> cascaded)
    {
        // Only a blueprint can take nothing but favorites with it: anything a category or component reaches,
        // it reaches through a blueprint first.
        if (cascaded.Keys.Any(kind => kind != RecordKind.Favorite))
        {
            return $"Other records need {subject}. If you continue, {Describe(cascaded)} will be deselected too. "
                   + $"That counts anything that uses {subject} through something else, like a blueprint "
                   + "nested inside another one.";
        }

        return cascaded[RecordKind.Favorite] == 1
            ? $"A favorite uses {subject}. If you continue, it'll be deselected too."
            : $"{Describe(cascaded)} use {subject}. If you continue, they'll be deselected too.";
    }

    /// <summary>"3 components, 2 blueprints and 1 favorite", in <see cref="RecordKind"/> order.</summary>
    private static string Describe(IReadOnlyDictionary<RecordKind, int> counts)
    {
        List<string> parts = [.. counts.OrderBy(pair => pair.Key).Select(pair => $"{pair.Value} {Noun(pair.Key, pair.Value)}")];

        return parts.Count == 1 ? parts[0] : $"{string.Join(", ", parts[..^1])} and {parts[^1]}";
    }

    private static string Noun(RecordKind kind, int count) => (kind, count == 1) switch
    {
        (RecordKind.Category, true) => "category",
        (RecordKind.Category, false) => "categories",
        (RecordKind.Component, true) => "component",
        (RecordKind.Component, false) => "components",
        (RecordKind.Blueprint, true) => "blueprint",
        (RecordKind.Blueprint, false) => "blueprints",
        (_, true) => "favorite",
        _ => "favorites"
    };
}
