using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.UI.Components.Dialogs;

namespace CraftingCalculator.UI.Components.Controls.Import;

/// <summary>
/// The import wizard's third step: how many of the chosen records share a name with records already in the target
/// dataset, and whether to keep those, replace them, or choose each one.
/// </summary>
public partial class ImportConflicts : BaseImportControl
{
    private string _description = "";

    protected override void OnInitialized()
    {
        // Built once rather than per render. Cancel clears the conflicts, and this control re-renders after its own click
        // handler before the page swaps it out, when there are no conflicts left for DescribeConflicts to describe.
        _description = TransferPrompts.DescribeConflicts(
            ImportState.TargetDatasetName,
            ImportState.Conflicts.GroupBy(conflict => conflict.Kind).ToDictionary(group => group.Key, group => group.Count()));
    }

    private Task KeepMineAsync() => ImportState.MergeAsync(new HashSet<RecordKey>());

    private Task ReplaceMineAsync() => ImportState.MergeAsync(ImportState.ConflictKeys);
}
