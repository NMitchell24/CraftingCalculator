using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// One record type's list, reached from the <see cref="Dataset" /> landing page. Rows open
/// <see cref="DatasetEditor" />, except while the list is in one of the bulk modes described by
/// <see cref="ListMode"/>, where a row tap feeds that mode instead.
/// </summary>
public partial class DatasetList : ComponentBase, IDisposable
{
    /// <summary>What a row tap does, driven by the Duplicate and Delete actions on the shell.</summary>
    private enum ListMode
    {
        /// <summary>A row tap opens the editor.</summary>
        Normal,

        /// <summary>The next row tap opens the editor on a copy of that record.</summary>
        Duplicate,

        /// <summary>Row taps toggle selection; the Delete action then deletes the selection.</summary>
        Delete
    }

    /// <summary>The <see cref="DataType"/> being listed, as its enum name.</summary>
    [Parameter] public string Type { get; set; } = "";

    [Inject] private IDatasetService DatasetService { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private DataType _type;
    private List<IBaseDataRecord> _records = [];
    private string _search = "";
    private ListMode _mode = ListMode.Normal;

    // Ids rather than records: ReloadAsync replaces every record instance, and these models have no
    // value equality, so a selection held as records would not survive a reload.
    private readonly HashSet<int> _selected = [];

    private List<IBaseDataRecord> FilteredRecords =>
        [.. _records.Where(record => string.IsNullOrWhiteSpace(_search)
            || (record.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false))];

    protected override async Task OnParametersSetAsync()
    {
        if (!Enum.TryParse(Type, ignoreCase: true, out _type))
        {
            Navigation.NavigateTo("/dataset");
            return;
        }

        await ReloadAsync();
    }

    private static string TitleFor(DataType type) => type switch
    {
        DataType.Blueprint => "Blueprints",
        DataType.Component => "Components",
        _ => "Categories"
    };

    private void ConfigureShell()
    {
        bool empty = _records.Count == 0;

        PageShellState.Configure(this, new PageShellConfig(TitleFor(_type))
        {
            BackHref = "/dataset",
            Actions =
            [
                new PageAction($"New {_type.GetDescription()}", Icons.Material.Filled.Add, CreateNewAsync),
                new PageAction("Duplicate", Icons.Material.Filled.ContentCopy, ToggleDuplicateModeAsync,
                    Disabled: empty, Active: _mode == ListMode.Duplicate),
                // Wired only while the mode is on, the one state the gesture means anything in. A hold
                // outside it is inert either way - the WebView delivers no click after a long press, so
                // that tap is lost whether or not a handler is attached.
                new PageAction("Delete", Icons.Material.Filled.Delete, ToggleDeleteModeAsync,
                    Disabled: empty, Active: _mode == ListMode.Delete,
                    OnLongPress: _mode == ListMode.Delete ? ExitDeleteModeAsync : null),
                new PageAction($"Delete all {TitleFor(_type)}", Icons.Material.Filled.DeleteForever,
                    DeleteAllAsync, Disabled: empty)
            ]
        });
    }

    private async Task ReloadAsync()
    {
        _records = await DatasetService.GetRecordsAsync(_type);

        // The actions carry both the mode and whether there is anything left to act on, so they are
        // re-declared on every reload rather than only when the mode changes.
        ConfigureShell();
        StateHasChanged();
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

    private Task ToggleDuplicateModeAsync()
    {
        if (_mode == ListMode.Duplicate)
        {
            SetMode(ListMode.Normal);
            return Task.CompletedTask;
        }

        SetMode(ListMode.Duplicate);
        Snackbar.Add($"Tap on a {_type.GetDescription()} to duplicate it", Severity.Info);

        return Task.CompletedTask;
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

    private void OnRowClick(IBaseDataRecord record)
    {
        switch (_mode)
        {
            case ListMode.Duplicate:
                // Deliberately not reset to Normal first: this navigates away, and coming back from
                // the editor lands on a fresh page instance whose mode is already Normal.
                Duplicate(record);
                break;

            case ListMode.Delete:
                // Remove reports whether the id was selected, so the toggle costs one lookup either way.
                if (!_selected.Remove(record.Id))
                {
                    _selected.Add(record.Id);
                }

                break;

            default:
                Edit(record);
                break;
        }
    }

    private string RowClass(IBaseDataRecord record) =>
        _mode == ListMode.Delete && _selected.Contains(record.Id) ? "list-row-selected" : "";

    private void Edit(IBaseDataRecord record) => Navigation.NavigateTo($"/dataset/{record.Type}/{record.Id}");

    private void Duplicate(IBaseDataRecord record) =>
        Navigation.NavigateTo($"/dataset/{record.Type}/0?copyFrom={record.Id}");

    private async Task DeleteAsync(IBaseDataRecord record)
    {
        if (!await DatasetPrompts.ConfirmDeleteAsync(DialogService, record))
        {
            return;
        }

        await DatasetService.DeleteRecordAsync(record);
        Snackbar.Add($"Deleted '{record.Name}'", Severity.Success);
        await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        List<IBaseDataRecord> selected = [.. _records.Where(record => _selected.Contains(record.Id))];

        if (selected.Count == 0)
        {
            SetMode(ListMode.Normal);
            return;
        }

        if (!await DatasetPrompts.ConfirmDeleteManyAsync(DialogService, _type, TitleFor(_type), selected.Count))
        {
            SetMode(ListMode.Normal);
            return;
        }

        await DatasetService.DeleteRecordsAsync(selected);
        await ReloadAsync();

        Snackbar.Add($"Deleted {selected.Count} {TitleFor(_type)}", Severity.Success);
        SetMode(ListMode.Normal);
    }

    private async Task DeleteAllAsync()
    {
        if (!await DatasetPrompts.ConfirmDeleteManyAsync(DialogService, _type, TitleFor(_type), _records.Count))
        {
            return;
        }

        await DatasetService.DeleteAllOfTypeAsync(_type);
        await ReloadAsync();

        Snackbar.Add($"Deleted all {TitleFor(_type)}", Severity.Success);
        SetMode(ListMode.Normal);
    }

    private static string CaptionFor(IBaseDataRecord record) => record switch
    {
        Component component => string.Format(FormatConstants.CurrencyFormat, component.Cost),
        Blueprint blueprint => BlueprintCaption(blueprint),
        _ => record.Description ?? ""
    };

    private static string BlueprintCaption(Blueprint blueprint)
    {
        int components = blueprint.Components.ComponentList.Count + blueprint.ChildBlueprints.BlueprintList.Count;
        string summary = $"{components} component{(components == 1 ? "" : "s")}";

        return string.IsNullOrWhiteSpace(blueprint.Category?.Name) ? summary : $"{blueprint.Category.Name} · {summary}";
    }

    public void Dispose() => PageShellState.Reset(this);
}
