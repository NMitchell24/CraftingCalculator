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
/// <see cref="DatasetEditor" />; the app bar creates a new record of this page's type.
/// </summary>
public partial class DatasetList : ComponentBase, IDisposable
{
    /// <summary>The <see cref="DataType"/> being listed, as its enum name.</summary>
    [Parameter] public string Type { get; set; } = "";

    [Inject] private IDatasetService DatasetService { get; set; } = null!;
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private DataType _type;
    private List<IBaseDataRecord> _records = [];
    private string _search = "";

    private List<IBaseDataRecord> FilteredRecords =>
        [.. _records.Where(r => string.IsNullOrWhiteSpace(_search)
            || (r.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false))];

    protected override async Task OnParametersSetAsync()
    {
        if (!Enum.TryParse(Type, ignoreCase: true, out _type))
        {
            Navigation.NavigateTo("/dataset");
            return;
        }

        AppBarState.Configure(this, new AppBarConfig(TitleFor(_type))
        {
            BackHref = "/dataset",
            PrimaryAction = new AppBarAction($"New {_type.GetDescription()}", Icons.Material.Filled.Add, CreateNewAsync)
        });

        await ReloadAsync();
    }

    private static string TitleFor(DataType type) => type switch
    {
        DataType.Blueprint => "Blueprints",
        DataType.Component => "Components",
        _ => "Categories"
    };

    private async Task ReloadAsync() => _records = await DatasetService.GetRecordsAsync(_type);

    private Task CreateNewAsync()
    {
        Navigation.NavigateTo($"/dataset/{_type}/0");
        return Task.CompletedTask;
    }

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

    public void Dispose() => AppBarState.Reset(this);
}
