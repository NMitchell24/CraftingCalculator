using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
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

    private HelpArticle? _article;
    private bool _loaded;
    private bool _scrollPending;

    private static string ContentsHref => $"/{HelpTopics.HelpRoot}";

    private static string HrefFor(HelpTopic topic) => $"/{HelpTopics.HelpRoot}/{topic.Id}";

    protected override async Task OnParametersSetAsync()
    {
        _article = TopicId is null ? null : await HelpService.GetArticleAsync(TopicId);
        _loaded = true;

        // Links inside an article route back into this same component, so Blazor updates the parameters
        // in place and the WebView keeps whatever scroll offset the previous page was left at - which
        // lands the reader partway down the page they just opened.
        _scrollPending = true;

        // The topic's own name is the H1 the Markdown opens with; the bar says which part of the app
        // this is, the way Settings does. TitleIsUserContent stays false: these are the app's own words.
        PageShellState.Configure(this, new PageShellConfig("Help")
        {
            BackHref = TopicId is null ? null : ContentsHref
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_scrollPending)
        {
            _scrollPending = false;

            // Not an unconditional reset to the top: pages link to each other's headings
            // (calculations.md#surplus), and HelpProcessor.RewriteLink keeps that fragment, so the
            // heading it names is where this render is supposed to land.
            await Js.InvokeVoidAsync("helpScroll.toTarget", new Uri(Navigation.Uri).Fragment.TrimStart('#'));
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public void Dispose() => PageShellState.Reset(this);
}
