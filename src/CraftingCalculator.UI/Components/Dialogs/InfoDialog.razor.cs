using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The detail behind one blueprint or component: what it is, what it is worth or costs, how long it
/// takes, and - for a blueprint - what it directly requires. Opened from the batch, the Components and
/// Surplus lists, and from the Crafting Steps tree, which adds the figures for the tapped step.
/// </summary>
public partial class InfoDialog
{
    private List<IBaseQuantityRecord> _parts = [];

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    /// <summary>The blueprint or component the dialog describes.</summary>
    [Parameter, EditorRequired] public IBaseDataRecord Record { get; set; } = null!;

    /// <summary>
    /// The Crafting Steps row the dialog was opened from, which adds the figures that apply to that
    /// step alone. Null when it was opened from a list, where no one step is in view.
    /// </summary>
    [Parameter] public BlueprintNode? Step { get; set; }

    private BlueprintModel? Blueprint => Record as BlueprintModel;

    private ComponentModel? Component => Record as ComponentModel;

    /// <summary>Opens the dialog for a blueprint or component picked from a list.</summary>
    public static Task<IDialogReference> ShowAsync(IDialogService dialogs, IBaseDataRecord record) =>
        ShowAsync(dialogs, record, null);

    /// <summary>Opens the dialog for one row of the Crafting Steps tree.</summary>
    public static Task<IDialogReference> ShowAsync(IDialogService dialogs, BlueprintNode step) =>
        ShowAsync(dialogs, step.Source, step);

    protected override void OnParametersSet() =>
        // Held in a field rather than read from a property in the markup: the panel's header renders
        // the count and its body the rows, so a property would rebuild the list twice per render.
        _parts = Blueprint is null ? [] : BlueprintPartProcessor.GetParts(Blueprint);

    private static Task<IDialogReference> ShowAsync(IDialogService dialogs, IBaseDataRecord record, BlueprintNode? step)
    {
        DialogParameters<InfoDialog> parameters = new()
        {
            { dialog => dialog.Record, record },
            { dialog => dialog.Step, step }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseOnEscapeKey = true };

        return dialogs.ShowAsync<InfoDialog>(record.Name ?? "", parameters, options);
    }

    private void Close() => MudDialog.Close();
}
