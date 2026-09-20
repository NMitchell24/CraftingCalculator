using System.Text;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.Infrastructure.Logging;

/// <summary>
/// Formats log entries for one category and hands them to its <see cref="FileLoggerProvider"/> for writing.
/// With a <see cref="LogRedactor"/> the entry is stripped of anything the user typed or named before it is
/// written; without one, exceptions are recorded verbatim.
/// </summary>
internal sealed class FileLogger(FileLoggerProvider provider, string categoryName, LogRedactor? redactor) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        // Level filtering is the logging framework's job (SetMinimumLevel / AddFilter at registration); this
        // sink accepts whatever the framework lets through.
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        StringBuilder entry = new();
        entry.Append($"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{logLevel}] {categoryName}: ")
            .AppendLine(formatter(state, exception));

        if (exception is not null)
        {
            AppendException(entry, exception);
        }

        // Applied last and to the whole entry: a rooted path can appear in the formatted message as readily as
        // in an exception, and collapsing it once here covers both.
        provider.Write(redactor is null ? entry.ToString() : redactor.CollapsePaths(entry.ToString()));
    }

    private void AppendException(StringBuilder entry, Exception exception)
    {
        if (redactor is null)
        {
            entry.AppendLine(exception.ToString());
            return;
        }

        // ToString() is never used when redacting: it renders every message in the chain, including the ones
        // this walk masks. Type and stack trace are our own code and are always kept in full.
        AppendRedactedException(entry, exception);
    }

    private static void AppendRedactedException(StringBuilder entry, Exception exception)
    {
        entry.Append(exception.GetType().FullName).Append(": ").AppendLine(LogRedactor.MaskMessage(exception));

        if (exception.StackTrace is { } stackTrace)
        {
            entry.AppendLine(stackTrace);
        }

        foreach (Exception inner in InnerExceptionsOf(exception))
        {
            entry.AppendLine("---> Inner exception:");
            AppendRedactedException(entry, inner);
        }
    }

    private static IEnumerable<Exception> InnerExceptionsOf(Exception exception) => exception switch
    {
        // InnerExceptions is the full set and its first element is InnerException, so taking both would
        // record that one twice.
        AggregateException aggregate => aggregate.InnerExceptions,
        { InnerException: { } inner } => [inner],
        _ => []
    };
}
