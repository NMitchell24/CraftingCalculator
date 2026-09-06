using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Library : ComponentBase, IDisposable
{
    [Parameter, SupplyParameterFromQuery(Name = "type")] public string? TypeQuery { get; set; }

    [Inject] private ILibraryService LibraryService { get; set; } = null!;
    [Inject] private IDatabaseAdminService DatabaseAdminService { get; set; } = null!;
    [Inject] private CalculatorState State { get; set; } = null!;
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private DataType _type = DataType.Recipe;
    private List<IBaseDataRecord> _records = [];
    private string _search = "";
    private bool _busy;

    private List<IBaseDataRecord> FilteredRecords =>
        [.. _records.Where(r => string.IsNullOrWhiteSpace(_search)
            || (r.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false))];

    protected override async Task OnInitializedAsync()
    {
        AppBarState.Configure(this, "Library",
        [
            new AppBarMenuItem("Delete all data", Icons.Material.Filled.DeleteForever, DeleteAllDataAsync)
        ]);

        // The editor sends the tab back on its return link, so reopening Library from an ingredient
        // lands on Ingredients rather than resetting to Recipes.
        if (Enum.TryParse(TypeQuery, ignoreCase: true, out DataType type))
        {
            _type = type;
        }

        await ReloadAsync();
    }

    private async Task ReloadAsync() => _records = await LibraryService.GetRecordsAsync(_type);

    private async Task OnTypeChangedAsync(DataType type)
    {
        _type = type;
        _search = "";
        await ReloadAsync();

        // Replace rather than push, so tab taps don't stack up history entries. This keeps the tab in
        // the URL for the Android hardware back button, which pops to this page's own history entry
        // rather than following the editor's return link.
        Navigation.NavigateTo($"/library?type={_type}", replace: true);
    }

    private void CreateNew() => Navigation.NavigateTo($"/library/{_type}/0");

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

    private async Task DeleteAllDataAsync()
    {
        DialogParameters parameters = new()
        {
            ["Message"] = "This removes every favorite, recipe, category, and ingredient from the app. "
                          + "It cannot be undone.",
            ["ConfirmWord"] = "DELETE",
            ["ConfirmText"] = "Delete everything"
        };

        IDialogReference dialogRef = await DialogService.ShowAsync<TypedConfirmDialog>("Delete Everything?", parameters);
        DialogResult? result = await dialogRef.Result;

        if (result is null or { Canceled: true })
        {
            return;
        }

        _busy = true;
        StateHasChanged();

        try
        {
            await DatabaseAdminService.DeleteAllDataAsync();

            // The batch on the Calculate screen holds Recipe models that no longer exist in the
            // database - left alone it would keep pricing out deleted recipes.
            State.Clear();

            await ReloadAsync();
        }
        finally
        {
            _busy = false;
        }

        Snackbar.Add("Deleted all data", Severity.Success);
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

        return string.IsNullOrWhiteSpace(recipe.Filter?.Name) ? summary : $"{recipe.Filter.Name} \u00b7 {summary}";
    }

    public void Dispose() => AppBarState.Reset(this);
}
