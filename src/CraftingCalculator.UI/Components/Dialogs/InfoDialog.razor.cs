using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
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
    // Record, or the full blueprint read in its place when Record is a summary.
    private IBaseDataRecord? _record;
    private List<IBaseQuantityRecord> _parts = [];
    private bool _loaded;
    private bool _partsExpanded;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;
    [Inject] private IFavoriteService FavoriteService { get; set; } = null!;

    /// <summary>
    /// The category, component or blueprint the dialog describes, or null when it describes a <see cref="Favorite"/>.
    /// A <see cref="BlueprintSummary"/> is read in full as the dialog opens.
    /// </summary>
    [Parameter] public IBaseDataRecord? Record { get; set; }

    /// <summary>The favorite the dialog describes, or null when it describes a <see cref="Record"/>.</summary>
    [Parameter] public BlueprintFavorite? Favorite { get; set; }

    /// <summary>
    /// The blueprints <see cref="Favorite"/> holds, and how many of each, or null to read them from the saved favorite
    /// as the dialog opens.
    /// </summary>
    [Parameter] public IReadOnlyList<BlueprintQuantity>? FavoriteBlueprints { get; set; }

    /// <summary>
    /// The Crafting Steps row the dialog was opened from, which adds the figures that apply to that
    /// step alone. Null when it was opened from a list, where no one step is in view.
    /// </summary>
    [Parameter] public BlueprintNode? Step { get; set; }

    private BlueprintModel? Blueprint => _record as BlueprintModel;

    private ComponentModel? Component => _record as ComponentModel;

    private string? Name => Record is not null ? Record.Name : Favorite?.Name;

    // Only a blueprint and a component name their kind. A category or a favorite is opened from a list that is
    // already labeled with it. Read off the type rather than the model, so a summary names it while it loads.
    private string? Kind => Record?.Type is DataType.Blueprint or DataType.Component ? Record.Type.GetDescription() : null;

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

    /// <summary>Opens the dialog for a saved favorite, reading the blueprints it holds.</summary>
    public static Task<IDialogReference> ShowAsync(IDialogService dialogs, BlueprintFavorite favorite) =>
        ShowAsync(dialogs, favorite.Name, new DialogParameters<InfoDialog> { { dialog => dialog.Favorite, favorite } });

    /// <summary>Opens the dialog for a favorite and the blueprints it holds.</summary>
    public static Task<IDialogReference> ShowAsync(
        IDialogService dialogs, BlueprintFavorite favorite, IReadOnlyList<BlueprintQuantity> blueprints) =>
        ShowAsync(dialogs, favorite.Name, new DialogParameters<InfoDialog>
        {
            { dialog => dialog.Favorite, favorite },
            { dialog => dialog.FavoriteBlueprints, blueprints }
        });

    protected override async Task OnInitializedAsync()
    {
        _record = Record;
        IReadOnlyList<BlueprintQuantity> favoriteBlueprints = FavoriteBlueprints ?? [];

        // A list row carries a summary, and the parts below need the whole blueprint. The summary stands in only when
        // the blueprint was deleted after the list was read.
        if (Record is BlueprintSummary summary)
        {
            if (await Task.Run(() => BlueprintService.GetBlueprintByIdAsync(summary.Id)) is { } blueprint)
            {
                _record = blueprint;
            }
        }
        else if (Favorite is { } favorite && FavoriteBlueprints is null)
        {
            favoriteBlueprints = await Task.Run(() => FavoriteService.GetBlueprintQuantitiesForFavoriteAsync(favorite));
        }

        // Held in a field rather than read from a property in the markup: the panel's header renders
        // the count and its body the rows, so a property would rebuild the list twice per render.
        _parts = Blueprint is not null
            ? BlueprintPartProcessor.GetParts(Blueprint)
            : [.. favoriteBlueprints.OrderBy(quantity => quantity.Name)];

        _loaded = true;
    }

    private static Task<IDialogReference> ShowAsync(IDialogService dialogs, string? title, DialogParameters<InfoDialog> parameters)
    {
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseOnEscapeKey = true };

        return dialogs.ShowAsync<InfoDialog>(title ?? "", parameters, options);
    }

    private void Close() => MudDialog.Close();
}
