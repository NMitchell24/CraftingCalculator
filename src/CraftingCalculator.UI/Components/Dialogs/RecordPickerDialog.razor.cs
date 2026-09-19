using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// Searches <see cref="Records" /> and adds them to <see cref="Target" /> or changes their quantity there in place.
/// Each change applies to <see cref="Target" /> immediately, so the dialog has no result.
/// </summary>
public partial class RecordPickerDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    /// <summary>The dialog's title.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>Every record the user can pick, in the order they are listed.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<IBaseDataRecord> Records { get; set; } = [];

    /// <summary>Where picked records go.</summary>
    [Parameter, EditorRequired] public IRecordPickerTarget Target { get; set; } = null!;

    /// <summary>The list whose category filter the dialog restores and remembers.</summary>
    [Parameter, EditorRequired] public FilterList FilterList { get; set; }

    private readonly Random _random = new();

    private readonly List<string> _noMatchPhrases =
    [
        "I can't find anything matching that search... it's not me, it's you.",
        "What?! There's nothing here!?",
        "I can't do that, Dave.",
        "Maybe if you try pressing other buttons it will work?",
        "Your search has achieved enlightenment by becoming empty.",
        "404: Results not found",
        "Hold on... checking again... nope, still nothing.",
        "I would show you results, but I forgot where I put them.",
        "The search hamsters are on their lunch break. Try again later.",
        "Your search didn't craft anything. Maybe you're missing the required materials?",
        "I rolled for loot. You get... nothing. Not even a tasty red snapper.",
        "Your search is uncraftable. Requires: 1 actual item name.",
        "That resource doesn't spawn here."
    ];

    private string GetNoMatchPhrase()
    {
        return _noMatchPhrases[_random.Next(_noMatchPhrases.Count)];
    }

    private RecordFilter _filter = RecordFilter.Empty;

    // Navigating here dismisses the dialog on its own: MudDialogProvider closes every dialog whose route changes.
    private static string GettingStartedHref => $"/{HelpTopics.HelpRoot}/{HelpTopics.GettingStartedTopicId}";

    private List<IBaseDataRecord> FilteredRecords => RecordFilterProcessor.Apply(Records, _filter);

    private void OnFilterChanged(RecordFilter filter) => _filter = filter;

    private static string RemoveLabel(IBaseDataRecord record) => $"Remove {record.Name}";

    private void Close() => MudDialog.Close();
}
