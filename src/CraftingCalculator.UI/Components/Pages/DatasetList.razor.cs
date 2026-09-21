using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// One record type's list, reached from the <see cref="Dataset" /> landing page. Each record is a card whose buttons
/// edit, copy or delete it; while the list is in Delete mode (<see cref="ListMode"/>), a tap on a card selects it.
/// </summary>
public partial class DatasetList : ComponentBase, IDisposable
{
    /// <summary>What a card tap does, driven by the Delete action on the shell.</summary>
    private enum ListMode
    {
        /// <summary>A card tap does nothing.</summary>
        Normal,

        /// <summary>Card taps toggle selection; the Delete action then deletes the selection.</summary>
        Delete
    }

    /// <summary>The <see cref="DataType"/> being listed, as its enum name.</summary>
    [Parameter] public string Type { get; set; } = "";

    [Inject] private IRecordService RecordService { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private CraftState CraftState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private ActionGuard Guard { get; set; } = null!;

    // One message for all three delete paths, so it says nothing about how many records were involved: the
    // list behind the dialog still shows them either way.
    private const string DeleteFailedMessage = "I couldn't finish that delete.";

    private DataType _type;
    private List<IBaseDataRecord> _records = [];
    private RecordFilter _filter = RecordFilter.Empty;
    private ListMode _mode = ListMode.Normal;

    // Ids rather than records: ReloadAsync replaces every record instance, and these models have no
    // value equality, so a selection held as records would not survive a reload.
    private readonly HashSet<int> _selected = [];

    // The Type the list was last loaded for. Parameters are set again whenever MainLayout re-renders, and
    // reloading for that would discard a Delete Mode selection and clear the filter out from under a search bar
    // still showing it.
    private string? _listedType;

    private List<IBaseDataRecord> FilteredRecords => RecordFilterProcessor.Apply(_records, _filter);

    protected override async Task OnParametersSetAsync()
    {
        if (Type == _listedType)
        {
            return;
        }

        if (!Enum.TryParse(Type, ignoreCase: true, out _type))
        {
            Navigation.NavigateTo("/dataset");
            return;
        }

        _listedType = Type;

        // The router reuses this instance when only {Type} changes, so the previous type's rows,
        // selection and mode would stay on screen for the length of the load below. SetMode clears
        // them and re-declares the shell against the new type before anything is awaited.
        _records = [];
        _filter = RecordFilter.Empty;
        SetMode(ListMode.Normal);

        await ReloadAsync();
    }

    private void OnFilterChanged(RecordFilter filter) => _filter = filter;

    private static FilterList FilterListFor(DataType type) => type switch
    {
        DataType.Blueprint => FilterList.Blueprints,
        DataType.Component => FilterList.Components,
        _ => FilterList.Categories
    };

    private static string TitleFor(DataType type) => type switch
    {
        DataType.Blueprint => "Blueprints",
        DataType.Component => "Components",
        _ => "Categories"
    };

    /// <summary>The type's noun agreeing with <paramref name="count"/>, for text that counts records.</summary>
    private static string NounFor(DataType type, int count) => count == 1 ? type.GetDescription() : TitleFor(type);

    private void ConfigureShell()
    {
        bool empty = _records.Count == 0;

        // TitleFor and DataType's description name the screen, where they are capitalized; an action label is a
        // sentence, where they are not.
        List<PageAction> actions =
        [
            new($"New {_type.GetDescription().ToLowerInvariant()}", Icons.Material.Filled.Add, CreateNewAsync),
            new("Delete", Icons.Material.Filled.Delete, ToggleDeleteModeAsync,
                Disabled: empty, Active: _mode == ListMode.Delete,
                OnLongPress: _mode == ListMode.Delete ? ExitDeleteModeAsync : null),

            new($"Delete all {TitleFor(_type).ToLowerInvariant()}", Icons.Material.Filled.DeleteForever,
                DeleteAllAsync, Disabled: empty)
        ];

        // Wired only while the mode is on, the one state the gesture means anything in. A hold
        // outside it is inert either way - the WebView delivers no click after a long press, so
        // that tap is lost whether or not a handler is attached.

        PageShellState.Configure(this, new PageShellConfig(TitleFor(_type))
        {
            ShowBack = true,
            Actions = actions
        });
    }

    private async Task ReloadAsync()
    {
        _records = await RecordService.GetRecordsAsync(_type);

        // The actions carry both the mode and whether there is anything left to act on, so they are
        // re-declared on every reload rather than only when the mode changes.
        ConfigureShell();
        StateHasChanged();
    }

    // A deleted record can be in the Craft batch, directly or nested, so the batch is read again along with the list.
    private async Task ReloadAfterDeleteAsync()
    {
        await CraftState.ReloadBlueprintsAsync();
        await ReloadAsync();
    }

    private void SetMode(ListMode mode)
    {
        _mode = mode;
        _selected.Clear();
        ConfigureShell();

        // ActionsBar owns the action's click, so its EventCallback renders that component and never
        // this page - the rows would otherwise keep the previous mode's appearance.
        StateHasChanged();
    }

    private async Task ToggleDeleteModeAsync()
    {
        // The second tap of the Delete action is what commits the selection, so the one action both
        // enters the mode and ends it.
        if (_mode == ListMode.Delete)
        {
            await DeleteSelectedAsync();
            return;
        }

        SetMode(ListMode.Delete);
        Snackbar.Add($"Tap on any {TitleFor(_type)} then tap delete again to delete them", Severity.Info);
    }

    /// <summary>Leaves Delete Mode with the selection discarded and nothing deleted.</summary>
    private Task ExitDeleteModeAsync()
    {
        SetMode(ListMode.Normal);
        return Task.CompletedTask;
    }

    private Task CreateNewAsync()
    {
        Navigation.NavigateTo($"/dataset/{_type}/0");
        return Task.CompletedTask;
    }

    private void ToggleSelection(IBaseDataRecord record)
    {
        if (_mode != ListMode.Delete)
        {
            return;
        }

        // Remove reports whether the id was selected, so the toggle costs one lookup either way.
        if (!_selected.Remove(record.Id))
        {
            _selected.Add(record.Id);
        }
    }

    private string RowClass(IBaseDataRecord record) =>
        _mode == ListMode.Delete && _selected.Contains(record.Id) ? "list-item-selected" : "";

    private void Edit(IBaseDataRecord record) => Navigation.NavigateTo($"/dataset/{record.Type}/{record.Id}");

    private void Copy(IBaseDataRecord record) =>
        Navigation.NavigateTo($"/dataset/{record.Type}/0?copyFrom={record.Id}");

    private async Task DeleteAsync(IBaseDataRecord record)
    {
        if (!await DatasetPrompts.ConfirmDeleteAsync(DialogService, record))
        {
            return;
        }

        bool deleted = await Guard.RunAsync(
            "DatasetList.Delete",
            DeleteFailedMessage,
            async () =>
            {
                await RecordService.DeleteRecordAsync(record);
                await ReloadAfterDeleteAsync();
            });

        if (deleted)
        {
            Snackbar.Add($"Deleted '{record.Name}'", Severity.Success);
        }
    }

    private async Task DeleteSelectedAsync()
    {
        List<IBaseDataRecord> selected = [.. _records.Where(record => _selected.Contains(record.Id))];

        // Short-circuits, so an empty selection leaves the mode without putting a confirmation for zero
        // records on screen.
        if (selected.Count == 0
            || !await DatasetPrompts.ConfirmDeleteManyAsync(DialogService, _type, NounFor(_type, selected.Count), selected.Count))
        {
            SetMode(ListMode.Normal);
            return;
        }

        bool deleted = await Guard.RunAsync(
            "DatasetList.DeleteSelected",
            DeleteFailedMessage,
            async () =>
            {
                await RecordService.DeleteRecordsAsync(selected);
                await ReloadAfterDeleteAsync();
            });

        if (deleted)
        {
            Snackbar.Add($"Deleted {selected.Count} {NounFor(_type, selected.Count)}", Severity.Success);
        }

        // Left whichever way it went: the selection is either deleted or reported as failed, and keeping the
        // mode on would leave the user staring at highlighted rows with a dialog telling them nothing happened.
        SetMode(ListMode.Normal);
    }

    private async Task DeleteAllAsync()
    {
        if (!await DatasetPrompts.ConfirmDeleteManyAsync(DialogService, _type, NounFor(_type, _records.Count), _records.Count))
        {
            return;
        }

        bool deleted = await Guard.RunAsync(
            "DatasetList.DeleteAll",
            DeleteFailedMessage,
            async () =>
            {
                await RecordService.DeleteAllOfTypeAsync(_type);
                await ReloadAfterDeleteAsync();
            });

        if (deleted)
        {
            Snackbar.Add($"Deleted all {TitleFor(_type)}", Severity.Success);
        }

        SetMode(ListMode.Normal);
    }

    private static List<string> DetailsFor(IBaseDataRecord record) =>
        string.IsNullOrWhiteSpace(record.Description) ? [] : [record.Description];

    public void Dispose() => PageShellState.Reset(this);
}
