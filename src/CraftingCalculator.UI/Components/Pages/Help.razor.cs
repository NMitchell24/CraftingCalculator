using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// The built-in help: a contents list at <c>/help</c>, and one page per topic at
/// <c>/help/{TopicId}</c>. Content is authored as Markdown under <c>docs/help</c> and rendered by
/// <see cref="IHelpService" />, so the same source serves the app, the project site and the wiki.
/// </summary>
public partial class Help : ComponentBase, IDisposable
{
    /// <summary>The <see cref="HelpTopic.Id" /> to show, or null for the contents list.</summary>
    [Parameter] public string? TopicId { get; set; }

    [Inject] private IHelpService HelpService { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    // Tags the replacement SwapInPlace issues, so the handler lets its own navigation through.
    private const string SwappedInPlace = "help-swapped-in-place";

    private HelpArticle? _article;
    private bool _loaded;
    private string _scrollTarget = "";
    private IDisposable? _navigationGuard;

    private static string ContentsHref => $"/{HelpTopics.HelpRoot}";

    private static string HrefFor(HelpTopic topic) => $"/{HelpTopics.HelpRoot}/{topic.Id}";

    protected override void OnInitialized() =>
        _navigationGuard = Navigation.RegisterLocationChangingHandler(SwapInPlace);

    protected override async Task OnParametersSetAsync()
    {
        _article = TopicId is null ? null : await HelpService.GetArticleAsync(TopicId);
        _loaded = true;

        // Pages link to each other's headings (calculations.md#surplus), and HelpProcessor.RewriteLink keeps
        // that fragment, so the heading it names is where this render is supposed to land.
        _scrollTarget = new Uri(Navigation.Uri).Fragment.TrimStart('#');

        // The topic's own name is the H1 the Markdown opens with; the bar says which part of the app
        // this is, the way Settings does.
        PageShellState.Configure(this, new PageShellConfig("Help")
        {
            ShowBack = TopicId is not null
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_scrollTarget.Length > 0)
        {
            string target = _scrollTarget;
            _scrollTarget = "";

            // MainLayout dispatches appScroll.restore for the same navigation before this runs, so the page is
            // already at the top when the heading is scrolled to.
            await Js.InvokeVoidAsync("helpScroll.toTarget", target);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Keeps Help to one history entry however many pages the reader opens, so back and Close help both
    /// return straight to the screen Help was opened from.
    /// </summary>
    private ValueTask SwapInPlace(LocationChangingContext context)
    {
        // Article links, the contents list and the footer button all push. Each is stopped and re-issued as a
        // replace, which comes back through here tagged. The entry beneath Help is never a help page, so a step
        // back is never caught. TargetLocation stays relative when the navigation came from a NavigateTo call.
        if (context.HistoryEntryState == SwappedInPlace
            || !HelpProcessor.IsHelpRoute(Navigation.ToAbsoluteUri(context.TargetLocation).AbsolutePath))
        {
            return ValueTask.CompletedTask;
        }

        context.PreventNavigation();
        Navigation.NavigateTo(context.TargetLocation,
            new NavigationOptions { ReplaceHistoryEntry = true, HistoryEntryState = SwappedInPlace });

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _navigationGuard?.Dispose();
        PageShellState.Reset(this);
    }
}
