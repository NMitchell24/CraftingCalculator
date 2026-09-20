using System.Buffers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace CraftingCalculator.Infrastructure.Logging;

/// <summary>
/// Strips a log entry of anything that could have come from the user: the folders the app stores their files
/// in (which carry dataset and record names, and the account name on Windows) and the values the framework
/// quotes into an exception message. Type names, stack traces and our own wording are left alone.
/// </summary>
/// <param name="roots">
/// The folders whose contents are the user's: a path starting at any of these is replaced whole.
/// </param>
internal sealed partial class LogRedactor(IReadOnlyList<string> roots)
{
    private const string Placeholder = "<path>";

    // Characters that end a path where one is written into a message: the quotes and brackets that wrap it,
    // the colon of the Java "/path/to/file: open failed" form, and the end of the line. Space is deliberately
    // NOT one of them - see PathEnd.
    private static readonly SearchValues<char> PathTerminators = SearchValues.Create(":'\",;)]>\r\n\t");

    // What tells a trailing word that is still part of a file name from one that is the sentence carrying it.
    // An export is named "{dataset}-{yyyyMMdd-HHmmss}.ccdata", so every name the app writes has some of these.
    private static readonly SearchValues<char> NameCharacters = SearchValues.Create("./\\-_0123456789");

    private readonly string[] _roots =
    [
        .. roots.Select(root => root.TrimEnd('/', '\\'))
            .Where(root => !string.IsNullOrWhiteSpace(root))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            // Longest first, so overlapping roots produce the same result whatever order they were passed in.
            .OrderByDescending(root => root.Length)
    ];

    /// <summary>
    /// <paramref name="text"/> with every path under one of the roots replaced by <c>&lt;path&gt;</c> and the
    /// file's extension, so what kind of file failed is still readable but its name is gone.
    /// </summary>
    internal string CollapsePaths(string text)
    {
        if (_roots.Length == 0)
        {
            return text;
        }

        StringBuilder? collapsed = null;
        int copied = 0;
        int index = 0;

        while (index < text.Length)
        {
            if (RootAt(text, index) is not { } root)
            {
                index++;
                continue;
            }

            collapsed ??= new StringBuilder(text.Length);
            collapsed.Append(text, copied, index - copied);

            int end = PathEnd(text, index + root.Length);
            collapsed.Append(Placeholder).Append(ExtensionOf(text.AsSpan(index, end - index)));
            copied = end;
            index = end;
        }

        if (collapsed is null)
        {
            return text;
        }

        collapsed.Append(text, copied, text.Length - copied);
        return collapsed.ToString();
    }

    /// <summary>The message of <paramref name="exception"/> with everything the framework quoted masked out.</summary>
    internal static string MaskMessage(Exception exception) =>
        // A SqliteException names tables, columns and constraints - the schema, which is ours and not the
        // user's - and masking it would throw away the only description of what the database refused.
        exception is SqliteException ? exception.Message : MaskQuoted(exception.Message);

    /// <summary>
    /// <paramref name="message"/> with every quoted segment replaced by <c>***</c>. A value the framework
    /// names is almost always one the user typed: a file name, a key, a column value.
    /// </summary>
    internal static string MaskQuoted(string message) =>
        DoubleQuoted().Replace(SingleQuoted().Replace(message, "'***'"), "\"***\"");

    // The letter guards are what keep "couldn't" and "wasn't" from opening a quote; a real quote is always
    // preceded and followed by a space, a bracket or the end of the message.
    [GeneratedRegex(@"(?<![A-Za-z])'[^'\r\n]*'(?![A-Za-z])")]
    private static partial Regex SingleQuoted();

    [GeneratedRegex("""(?<![A-Za-z])"[^"\r\n]*"(?![A-Za-z])""")]
    private static partial Regex DoubleQuoted();

    /// <summary>
    /// Where the path whose root ends at <paramref name="start"/> ends: the first character that cannot follow
    /// a path in a sentence, less any trailing words that read as prose.
    /// </summary>
    private static int PathEnd(string text, int start)
    {
        int end = start;
        while (end < text.Length && !PathTerminators.Contains(text[end]))
        {
            end++;
        }

        // A space cannot end the path, because a dataset name can contain one and stopping at the first would
        // leave half an export's file name in the log. So the run above takes the rest of the sentence with it
        // and the words that are plainly not part of a name are handed back here, one at a time.
        while (true)
        {
            while (end > start && text[end - 1] == '.')
            {
                // A path ending a sentence would otherwise take the full stop with it, and the extension would
                // then be read from a segment ending in a dot.
                end--;
            }

            int wordStart = end;
            while (wordStart > start && text[wordStart - 1] != ' ')
            {
                wordStart--;
            }

            // Backtracked as far as the root, which is a path whatever follows it.
            if (wordStart == start || text.AsSpan(wordStart, end - wordStart).IndexOfAny(NameCharacters) >= 0)
            {
                return end;
            }

            end = wordStart;
            while (end > start && text[end - 1] == ' ')
            {
                end--;
            }
        }
    }

    private string? RootAt(string text, int index) =>
        _roots.FirstOrDefault(root => text.AsSpan(index).StartsWith(root, StringComparison.OrdinalIgnoreCase));

    private static ReadOnlySpan<char> ExtensionOf(ReadOnlySpan<char> path)
    {
        // Only the last segment, or "/data/user/0/com.sterlingtp.craftingcalculator/files" would be read as
        // having the extension ".craftingcalculator".
        ReadOnlySpan<char> fileName = path[(path.LastIndexOfAny('/', '\\') + 1)..];
        int dot = fileName.LastIndexOf('.');

        return dot < 0 ? [] : fileName[dot..];
    }
}
