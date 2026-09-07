using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// The Library landing page: one card per record type showing how many of it exist, each opening that
/// type's list. The lists themselves live in <see cref="LibraryList" />.
/// </summary>
public partial class Library : ComponentBase, IDisposable
{
    /// <summary>One row of the landing page - a record type, its heading, and its current count.</summary>
    private sealed record LibrarySection(DataType Type, string Title, string Caption);

    [Inject] private ILibraryService LibraryService { get; set; } = null!;
    [Inject] private IDatabaseAdminService DatabaseAdminService { get; set; } = null!;
    [Inject] private CalculatorState State { get; set; } = null!;
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private List<LibrarySection> Sections { get; set; } = [];
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        AppBarState.Configure(this, new AppBarConfig("Dataset")
        {
            MenuItems =
            [
                new AppBarMenuItem("Delete all data", Icons.Material.Filled.DeleteForever, DeleteAllDataAsync)
            ]
        });

        await ReloadAsync();
    }

    // Ordered the way the records have to be created: a blueprint needs components, and an component is
    // filed under a category, so the landing page reads top to bottom as the path a new user takes.
    // Both noun forms are spelled out rather than derived from the heading: "Categories" does not
    // singularise by trimming an s, and it does not pluralise by adding one either.
    private static readonly (DataType Type, string Title, string Singular, string Plural)[] SectionSpecs =
    [
        (DataType.Category, "Categories", "category", "categories"),
        (DataType.Component, "Components", "component", "components"),
        (DataType.Blueprint, "Blueprints", "blueprint", "blueprints")
    ];

    // Counting means loading each type in full, since ILibraryService exposes no count. That is the same
    // work the list pages already do and the data is local SQLite, so it is not worth a service method
    // until one of these lists is large enough to notice.
    private async Task ReloadAsync()
    {
        List<LibrarySection> sections = [];

        foreach ((DataType type, string title, string singular, string plural) in SectionSpecs)
        {
            int count = (await LibraryService.GetRecordsAsync(type)).Count;
            sections.Add(new LibrarySection(type, title, $"{count} {(count == 1 ? singular : plural)}"));
        }

        Sections = sections;
    }

    private void OpenList(DataType type) => Navigation.NavigateTo($"/library/{type}");

    private async Task DeleteAllDataAsync()
    {
        DialogParameters parameters = new()
        {
            ["Message"] = "This removes every favorite, blueprint, category, and component from the app. "
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

            // The batch on the Calculate screen holds Blueprint models that no longer exist in the
            // database - left alone it would keep pricing out deleted blueprints.
            State.Clear();

            await ReloadAsync();
        }
        finally
        {
            _busy = false;
        }

        Snackbar.Add("Deleted all data", Severity.Success);
    }

    public void Dispose() => AppBarState.Reset(this);
}
