using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls.Import;

/// <summary>
/// The import wizard's fourth step: one panel per kind of record with conflicts, where every conflict the user taps
/// replaces the record of the same name in the target dataset, and every other conflict keeps it. A panel's header
/// button picks or unpicks every conflict in the panel.
/// </summary>
public partial class ImportChooseEach : BaseImportControl
{
    // Matches .transfer-row's height in app.css, which is fixed so Virtualize can position rows exactly.
    private const float RowHeight = 56;

    /// <summary>Raised when the user taps Import Selected.</summary>
    [Parameter] public EventCallback OnImport { get; set; }

    private List<(TransferRecordKinds.Display Panel, List<ImportConflict> Rows)> _panels = [];

    private string? CycleMessage => ImportState.CycleNames switch
    {
        [] => null,
        [var name] => $"Those picks would nest '{name}' inside itself, so nothing was imported. Change a pick, then "
                      + "import again.",
        var names => $"Those picks would nest {string.Join(", ", names.Take(names.Count - 1).Select(Quoted))} and "
                     + $"{Quoted(names[^1])} inside each other, so nothing was imported. Change a pick, then import again."
    };

    protected override void OnInitialized()
    {
        // Sorted once rather than per render: importing a dataset back into itself conflicts on every record. The
        // conflicts only change on another step, and the page renders a new control when the step changes.
        ILookup<RecordKind, ImportConflict> byKind = ImportState.Conflicts.ToLookup(conflict => conflict.Kind);

        _panels =
        [
            .. TransferRecordKinds.All
                .Where(panel => byKind.Contains(panel.Kind))
                .Select(panel => (panel, byKind[panel.Kind].OrderBy(conflict => conflict.Name, StringComparer.OrdinalIgnoreCase).ToList()))
        ];
    }

    private static RecordKey KeyOf(ImportConflict conflict) => new(conflict.Kind, conflict.IncomingId);

    private bool IsPicked(ImportConflict conflict) => ImportState.Replace.Contains(KeyOf(conflict));

    private string RowClass(ImportConflict conflict) =>
        IsPicked(conflict) ? "transfer-row transfer-row-selected" : "transfer-row";

    private void ToggleAll(List<ImportConflict> rows, SelectionState state)
    {
        IEnumerable<RecordKey> keys = rows.Select(KeyOf);

        if (state == SelectionState.All)
        {
            ImportState.KeepAll(keys);
        }
        else
        {
            ImportState.ReplaceAll(keys);
        }
    }

    private static string Quoted(string name) => $"'{name}'";
}
