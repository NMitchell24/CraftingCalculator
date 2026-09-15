using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The detail behind one record: what it is, what it is worth or costs, how long it takes, and - for a
/// blueprint or a favorite - what it directly holds. Opened from the batch, the Components and Surplus lists,
/// the export selection panels, and from the Crafting Steps tree, which adds the figures for the tapped step.
/// </summary>
public partial class InfoDialog
{
    private List<IBaseQuantityRecord> _parts = [];
    private bool _partsExpanded;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    /// <summary>The category, component or blueprint the dialog describes, or null when it describes a <see cref="Favorite"/>.</summary>
    [Parameter] public IBaseDataRecord? Record { get; set; }

    /// <summary>The favorite the dialog describes, or null when it describes a <see cref="Record"/>.</summary>
    [Parameter] public BlueprintFavorite? Favorite { get; set; }

    /// <summary>The blueprints <see cref="Favorite"/> holds, and how many of each.</summary>
    [Parameter] public IReadOnlyList<BlueprintQuantity> FavoriteBlueprints { get; set; } = [];

    /// <summary>
    /// The Crafting Steps row the dialog was opened from, which adds the figures that apply to that
    /// step alone. Null when it was opened from a list, where no one step is in view.
    /// </summary>
    [Parameter] public BlueprintNode? Step { get; set; }

    private BlueprintModel? Blueprint => Record as BlueprintModel;

    private ComponentModel? Component => Record as ComponentModel;

    private string? Name => Record is not null ? Record.Name : Favorite?.Name;

    private string PartsLabel => Favorite is null ? "Components" : "Blueprints";

    private string PartsCaretClass => _partsExpanded ? "panel-caret panel-caret-expanded" : "panel-caret";

    /// <summary>Opens the dialog for a category, component or blueprint picked from a list.</summary>
    public static Task<IDialogReference> ShowAsync(IDialogService dialogs, IBaseDataRecord record) =>
        ShowAsync(dialogs, record.Name, new DialogParameters<InfoDialog> { { dialog => dialog.Record, record } });

    /// <summary>Opens the dialog for one row of the Crafting Steps tree.</summary>
    public static Task<IDialogReference> ShowAsync(IDialogService dialogs, BlueprintNode step) =>
        ShowAsync(dialogs, step.Source.Name, new DialogParameters<InfoDialog>
        {
            { dialog => dialog.Record, step.Source },
            { dialog => dialog.Step, step }
        });

    /// <summary>Opens the dialog for a favorite and the blueprints it holds.</summary>
    public static Task<IDialogReference> ShowAsync(
        IDialogService dialogs, BlueprintFavorite favorite, IReadOnlyList<BlueprintQuantity> blueprints) =>
        ShowAsync(dialogs, favorite.Name, new DialogParameters<InfoDialog>
        {
            { dialog => dialog.Favorite, favorite },
            { dialog => dialog.FavoriteBlueprints, blueprints }
        });

    protected override void OnParametersSet() =>
        // Held in a field rather than read from a property in the markup: the panel's header renders
        // the count and its body the rows, so a property would rebuild the list twice per render.
        _parts = Blueprint is not null
            ? BlueprintPartProcessor.GetParts(Blueprint)
            : [.. FavoriteBlueprints.OrderBy(quantity => quantity.Name)];

    private static Task<IDialogReference> ShowAsync(IDialogService dialogs, string? title, DialogParameters<InfoDialog> parameters)
    {
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseOnEscapeKey = true };

        return dialogs.ShowAsync<InfoDialog>(title ?? "", parameters, options);
    }

    private void Close() => MudDialog.Close();
}
