using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
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

    private RecordFilter _filter = RecordFilter.Empty;

    // Each row names its record's type only when Records mixes types; a list of one type would repeat the same word
    // on every row.
    private bool _showType;

    private List<IBaseDataRecord> FilteredRecords => RecordFilterProcessor.Apply(Records, _filter);

    protected override void OnParametersSet() =>
        _showType = Records.Select(record => record.Type).Distinct().Skip(1).Any();

    private void OnFilterChanged(RecordFilter filter) => _filter = filter;

    private void Close() => MudDialog.Close();
}
