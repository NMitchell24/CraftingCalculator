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
/// One record type's list, reached from the <see cref="Library" /> landing page. Rows open
/// <see cref="LibraryEditor" />; the app bar creates a new record of this page's type.
/// </summary>
public partial class LibraryList : ComponentBase, IDisposable
{
    /// <summary>The <see cref="DataType"/> being listed, as its enum name.</summary>
    [Parameter] public string Type { get; set; } = "";

    [Inject] private ILibraryService LibraryService { get; set; } = null!;
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
            Navigation.NavigateTo("/library");
            return;
        }

        AppBarState.Configure(this, new AppBarConfig(TitleFor(_type))
        {
            BackHref = "/library",
            PrimaryAction = new AppBarAction($"New {_type.GetDescription()}", Icons.Material.Filled.Add, CreateNewAsync)
        });

        await ReloadAsync();
    }

    private static string TitleFor(DataType type) => type switch
    {
        DataType.Recipe => "Blueprints",
        DataType.Ingredient => "Components",
        _ => "Categories"
    };

    private async Task ReloadAsync() => _records = await LibraryService.GetRecordsAsync(_type);

    private Task CreateNewAsync()
    {
        Navigation.NavigateTo($"/library/{_type}/0");
        return Task.CompletedTask;
    }

    private void Edit(IBaseDataRecord record) => Navigation.NavigateTo($"/library/{record.Type}/{record.Id}");

    private void Duplicate(IBaseDataRecord record) =>
        Navigation.NavigateTo($"/library/{record.Type}/0?copyFrom={record.Id}");

    private async Task DeleteAsync(IBaseDataRecord record)
    {
        if (!await LibraryPrompts.ConfirmDeleteAsync(DialogService, record))
        {
            return;
        }

        await LibraryService.DeleteRecordAsync(record);
        Snackbar.Add($"Deleted '{record.Name}'", Severity.Success);
        await ReloadAsync();
    }

    private static string CaptionFor(IBaseDataRecord record) => record switch
    {
        Ingredient ingredient => string.Format(FormatConstants.CurrencyFormat, ingredient.Cost),
        Recipe recipe => RecipeCaption(recipe),
        _ => record.Description ?? ""
    };

    private static string RecipeCaption(Recipe recipe)
    {
        int components = recipe.Ingredients.IngredientList.Count + recipe.ChildRecipes.RecipeList.Count;
        string summary = $"{components} component{(components == 1 ? "" : "s")}";

        return string.IsNullOrWhiteSpace(recipe.Filter?.Name) ? summary : $"{recipe.Filter.Name} · {summary}";
    }

    public void Dispose() => AppBarState.Reset(this);
}
