using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Controls;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Craft : ComponentBase, IRecordPickerTarget, IDisposable
{
    private enum CraftView
    {
        Materials,
        Steps,
        Surplus
    }

    [CascadingParameter] private Breakpoint Breakpoint { get; set; }

    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IClipboardService ClipboardService { get; set; } = null!;
    [Inject] private IFavoriteService FavoriteService { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;

    /// <summary>One tap of a batch row's + or - in the normal mode.</summary>
    private const long SingleStep = 1;

    /// <summary>One tap of a batch row's + or - while the x10 mode is on, or of its own +10 / -10 buttons.</summary>
    private const long BulkStep = 10;

    private CraftView _view = CraftView.Materials;
    private List<BlueprintFavorite> _favorites = [];
    private long _stepSize = SingleStep;

    private int _favoriteSelectNonce;

    // The IsCompact the shell's actions were last declared for; null until the first ConfigureShell.
    private bool? _shellCompact;

    // Drives the x10 mode (compact keeps it on the shell; wider viewports put +10 / -10 on every row
    // instead) and the blueprint picker's FullScreen choice. The pane layout itself is a pure CSS
    // media query (app.css), since it needs both width and height to tell a landscape phone apart from a
    // real tablet.
    private bool IsCompact => Breakpoint == Breakpoint.Xs;

    private long? RowBulkStep => IsCompact ? null : BulkStep;

    // Reflects CraftState rather than holding a selection of its own, so clearing the batch or loading a
    // favorite from anywhere else moves the select with it.
    private int? SelectedFavoriteId =>
        _favorites.FirstOrDefault(favorite => favorite.Name == State.LoadedFavoriteName)?.Id;

    private string FavoriteSelectLabel => _favorites.Count == 0 ? "No Favorites" : "Load Favorite";

    // The Load Favorite select is rebuilt rather than updated: MudSelect holds the value it set itself
    // and does not reliably take a new one back from its Value parameter, which showed up as the field
    // naming a favorite that was never loaded. The id covers a selection made elsewhere - saving a new
    // favorite, clearing the batch; the nonce covers the case the id cannot, where backing out of the
    // replace confirm leaves SelectedFavoriteId exactly where it already was.
    private (int Nonce, int? FavoriteId) FavoriteSelectKey => (_favoriteSelectNonce, SelectedFavoriteId);

    protected override async Task OnInitializedAsync()
    {
        State.Changed += OnStateChanged;
        ConfigureShell();

        await ReloadFavoritesAsync();
    }

    protected override void OnParametersSet()
    {
        // The cascaded Breakpoint is this page's only parameter. Configure re-renders MainLayout, which cascades
        // the Breakpoint again, so the shell is re-declared only when the viewport crosses the compact boundary.
        if (_shellCompact == IsCompact)
        {
            return;
        }

        // Leaving compact takes the x10 action off the shell, so the mode goes with it: the stepper would
        // otherwise keep moving by ten beside the rows' own +10 / -10 buttons, with nothing on screen to turn it off.
        _stepSize = SingleStep;
        ConfigureShell();
    }

    // Re-declared rather than configured once, because the x10 action carries both the mode it is in and
    // whether the batch has anything to step, and exists only on a compact viewport.
    private void ConfigureShell()
    {
        _shellCompact = IsCompact;

        List<PageAction> actions =
        [
            new("Add blueprints", Icons.Material.Filled.Add, OpenPickerAsync),
            new("Clear selection", Icons.Material.Filled.Clear, ClearBatchAsync),
            new("Save as favorite", Icons.Material.Filled.Save, SaveAsFavoriteAsync)
        ];

        if (IsCompact)
        {
            actions.Add(new PageAction("Step by 10", StepIcons.TimesTen, ToggleStepSizeAsync,
                Disabled: State.BlueprintQuantities.Count == 0, Active: _stepSize == BulkStep));
        }

        PageShellState.Configure(this, new PageShellConfig("Craft") { Actions = actions });
    }

    private void OnStateChanged()
    {
        // An emptied batch drops the mode rather than leaving it armed for whatever is added next: the
        // action is disabled while there is nothing to step, so there would be no way to turn it off.
        if (State.BlueprintQuantities.Count == 0)
        {
            _stepSize = SingleStep;
        }

        ConfigureShell();
        StateHasChanged();
    }

    /// <summary>Switches the batch steppers between stepping by one and stepping by ten.</summary>
    private Task ToggleStepSizeAsync()
    {
        _stepSize = _stepSize == SingleStep ? BulkStep : SingleStep;
        ConfigureShell();

        if (_stepSize == BulkStep)
        {
            Snackbar.Add("Stepping by 10. Tap x10 again to go back to 1", Severity.Info);
        }

        // ActionsBar owns the action's click, so its EventCallback renders that component and never this
        // page - the steppers would otherwise keep the previous mode's icons.
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task ReloadFavoritesAsync()
    {
        _favorites = await FavoriteService.GetAllFavoritesAsync();

        // Rendered here rather than left to the caller: ActionsBar owns the Save action's click, so its
        // EventCallback renders that component and never this page.
        StateHasChanged();
    }

    private async Task OnFavoriteSelectedAsync(int? favoriteId)
    {
        _favoriteSelectNonce++;

        if (_favorites.FirstOrDefault(favorite => favorite.Id == favoriteId) is { } selected)
        {
            await FavoritePrompts.LoadAsync(DialogService, Snackbar, State, selected);
        }
    }

    private void OnViewChanged(CraftView view) => _view = view;

    private async Task OpenPickerAsync()
    {
        List<BlueprintModel> blueprints = await BlueprintService.GetAllBlueprintsAsync();

        DialogOptions options = new()
        {
            FullScreen = IsCompact,
            MaxWidth = MaxWidth.Small,
            CloseOnEscapeKey = true
        };

        // Every pick goes straight into the batch through this page, so there is no result to await.
        DialogParameters<RecordPickerDialog> parameters = new()
        {
            { dialog => dialog.Title, "Add blueprints" },
            { dialog => dialog.Records, blueprints },
            { dialog => dialog.Target, this },
            { dialog => dialog.FilterList, FilterList.CraftPicker }
        };

        await DialogService.ShowAsync<RecordPickerDialog>("Add blueprints", parameters, options);
    }

    // The picker lists only blueprints and is only handed entries Find returned, so the casts below cannot fail.
    public IBaseQuantityRecord? Find(IBaseDataRecord record) =>
        State.BlueprintQuantities.FirstOrDefault(entry => entry.Name == record.Name);

    public Task AddAsync(IBaseDataRecord record)
    {
        State.Add((BlueprintModel)record);
        return Task.CompletedTask;
    }

    public Task StepAsync(IBaseQuantityRecord entry, long step)
    {
        State.Step((BlueprintQuantity)entry, step);
        return Task.CompletedTask;
    }

    public Task SetQuantityAsync(IBaseQuantityRecord entry, long quantity)
    {
        State.SetQuantity((BlueprintQuantity)entry, quantity);
        return Task.CompletedTask;
    }

    private Task ClearBatchAsync()
    {
        State.Clear();
        return Task.CompletedTask;
    }

    private async Task CopyMaterialsAsync()
    {
        string text = string.Join(Environment.NewLine, State.TotalComponents.Select(componentQuantity => componentQuantity.DisplayName));
        await ClipboardService.SetTextAsync(text);
        Snackbar.Add("Copied components to clipboard", Severity.Success);
    }

    private async Task CopySurplusAsync()
    {
        string text = string.Join(Environment.NewLine, State.SurplusStock.Select(blueprintQuantity => blueprintQuantity.DisplayName));
        await ClipboardService.SetTextAsync(text);
        Snackbar.Add("Copied surplus to clipboard", Severity.Success);
    }

    private async Task SaveAsFavoriteAsync()
    {
        if (await FavoritePrompts.SaveBatchAsync(DialogService, Snackbar, State) is not null)
        {
            await ReloadFavoritesAsync();
        }
    }

    public void Dispose()
    {
        State.Changed -= OnStateChanged;
        PageShellState.Reset(this);
    }
}
