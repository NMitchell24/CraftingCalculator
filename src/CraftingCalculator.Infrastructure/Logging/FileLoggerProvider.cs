using CraftingCalculator.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.Infrastructure.Logging;

/// <summary>
/// Logging provider that appends entries to a rotating set of files under one directory, so diagnostics from a
/// shipped Release build survive on the device and can be read back by the user. Writing is best-effort: a
/// failure to log never throws into the caller.
/// </summary>
/// <param name="logDirectory">The folder the log files live in. Created on the first write.</param>
/// <param name="redactor">
/// Applied to every entry before it is written, or null to write exceptions verbatim.
/// </param>
internal sealed class FileLoggerProvider(string logDirectory, LogRedactor? redactor) : ILoggerProvider, IDiagnosticLog
{
    private const string LogFileName = "crafting-calculator.log";

    private const string ArchiveFileNameFormat = "crafting-calculator.{0}.log";

    internal const long MaxFileSizeBytes = 128 * 1024;

    // .1.log (newest) and .2.log (oldest), so the device holds at most 3 files / ~384 KB, forever.
    internal const int RetainedArchives = 2;

    private readonly Lock _gate = new();

    public string FilePath { get; } = Path.Combine(logDirectory, LogFileName);

    public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName, redactor);

    public void Dispose()
    {
        // Nothing held open: each write opens and closes the file, so a crash cannot lose a buffered entry
        // and there is no stream to release here.
    }

    public string ReadAll()
    {
        try
        {
            // Same gate as Write, so a concurrent append cannot produce a torn read.
            lock (_gate)
            {
                string[] paths =
                [
                    .. Enumerable.Range(1, RetainedArchives).Reverse().Select(ArchivePath),
                    FilePath
                ];

                return string.Concat(paths.Select(path => File.Exists(path) ? File.ReadAllText(path) : string.Empty));
            }
        }
        catch
        {
            // Diagnostics must never themselves crash the app.
            return string.Empty;
        }
    }

    /// <summary>Full path of archive number <paramref name="index"/>, 1 being the most recently rotated out.</summary>
    internal string ArchivePath(int index) =>
        Path.Combine(logDirectory, string.Format(ArchiveFileNameFormat, index));

    /// <summary>Appends one formatted entry, rotating the files first when the current one is at the size cap.</summary>
    internal void Write(string entry)
    {
        try
        {
            lock (_gate)
            {
                Directory.CreateDirectory(logDirectory);
                RotateIfNeeded();
                File.AppendAllText(FilePath, entry);
            }
        }
        catch
        {
            // Diagnostics must never themselves crash the app.
        }
    }

    private void RotateIfNeeded()
    {
        if (new FileInfo(FilePath) is not { Exists: true, Length: >= MaxFileSizeBytes })
        {
            return;
        }

        // Walked from the oldest end so each move lands on a slot that has already been vacated; the first
        // move overwrites the oldest archive, which is how the total is bounded.
        for (int index = RetainedArchives; index >= 1; index--)
        {
            string source = index == 1 ? FilePath : ArchivePath(index - 1);

            if (File.Exists(source))
            {
                File.Move(source, ArchivePath(index), overwrite: true);
            }
        }
    }
}
