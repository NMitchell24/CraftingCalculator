using CraftingCalculator.Application.BusinessLogic.Processors;
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

    private RecordFilter _filter = RecordFilter.Empty;

    private List<IBaseDataRecord> FilteredRecords => RecordFilterProcessor.Apply(Records, _filter);

    private void OnFilterChanged(RecordFilter filter) => _filter = filter;

    private static string RemoveLabel(IBaseDataRecord record) => $"Remove {record.Name}";

    private void Close() => MudDialog.Close();
}
