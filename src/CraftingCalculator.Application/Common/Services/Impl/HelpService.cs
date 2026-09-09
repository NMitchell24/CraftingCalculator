using System.Reflection;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

/// <inheritdoc cref="IHelpService" />
public class HelpService : IHelpService
{
    // The Markdown is authored in docs/help and linked into this assembly as an embedded resource (see
    // CraftingCalculator.Application.csproj), so one set of files serves the app, the project site and
    // the wiki. LogicalName in that csproj entry is what pins this prefix.
    private const string ResourceNamespace = "CraftingCalculator.Application.Help.";

    /// <summary>Where the icons the pages reference as <c>assets/&lt;name&gt;.svg</c> are embedded.</summary>
    private const string AssetNamespace = ResourceNamespace + "Assets.";

    private static readonly Assembly ContentAssembly = typeof(HelpService).Assembly;

    public async Task<HelpArticle?> GetArticleAsync(string? topicId)
    {
        if (HelpProcessor.Find(topicId) is not { } topic)
        {
            return null;
        }

        await using Stream? stream = ContentAssembly.GetManifestResourceStream($"{ResourceNamespace}{topic.FileName}");

        if (stream is null)
        {
            return null;
        }

        using StreamReader reader = new(stream);
        string markdown = await reader.ReadToEndAsync();

        return new HelpArticle(topic, HelpProcessor.Render(markdown, ReadIcon));
    }

    private static string? ReadIcon(string fileName)
    {
        using Stream? stream = ContentAssembly.GetManifestResourceStream($"{AssetNamespace}{fileName}");

        if (stream is null)
        {
            return null;
        }

        using StreamReader reader = new(stream);

        return reader.ReadToEnd();
    }
}
