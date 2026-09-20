using AwesomeAssertions;
using CraftingCalculator.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.Infrastructure.UnitTests.Logging;

[TestFixture]
public class FileLoggerProviderTests
{
    // One padded entry is a little over this; the bounds below allow for that overshoot on each file.
    private const int PaddingBytes = 4096;

    private string _logDirectory = null!;
    private FileLoggerProvider _provider = null!;

    [SetUp]
    public void Setup()
    {
        _logDirectory = Path.Combine(Path.GetTempPath(), $"cc_logs_{Guid.NewGuid():N}");
        _provider = new FileLoggerProvider(_logDirectory, redactor: null);
    }

    [TearDown]
    public void TearDown()
    {
        _provider.Dispose();

        if (Directory.Exists(_logDirectory))
        {
            Directory.Delete(_logDirectory, recursive: true);
        }
    }

    [Test]
    public void Log_WritesLevelCategoryAndMessage()
    {
        ILogger logger = _provider.CreateLogger("CraftingCalculator.UI.SomePage");

        logger.LogInformation("Session started");

        string content = File.ReadAllText(_provider.FilePath);
        content.Should().Contain("[Information]");
        content.Should().Contain("CraftingCalculator.UI.SomePage");
        content.Should().Contain("Session started");
    }

    [Test]
    public void Log_WithException_WritesStackTraceAndInnerException()
    {
        ILogger logger = _provider.CreateLogger("test");
        Exception thrown = CaptureThrownException();

        logger.LogError(thrown, "Something failed");

        string content = File.ReadAllText(_provider.FilePath);
        content.Should().Contain(nameof(CaptureThrownException), "the stack trace must be preserved");
        content.Should().Contain("the inner reason", "inner exceptions must be preserved");
    }

    [Test]
    public void Log_WithoutRedactor_WritesTheExceptionMessageRaw()
    {
        ILogger logger = _provider.CreateLogger("test");

        logger.LogError(new InvalidOperationException("the dataset 'Valheim' is gone"), "Something failed");

        // Debug builds register the provider without a redactor so development logs stay readable.
        File.ReadAllText(_provider.FilePath).Should().Contain("'Valheim'");
    }

    [Test]
    public void Log_WithRedactor_MasksQuotedValuesThroughTheWholeExceptionChain()
    {
        FileLoggerProvider provider = new(_logDirectory, new LogRedactor([]));
        ILogger logger = provider.CreateLogger("test");
        AggregateException thrown = new(
            new InvalidOperationException("outer names 'Bronze Axe'",
                new ArgumentException("inner names 'Bronze Ingot'")),
            new IOException("sibling names 'Tin Ore'"));

        logger.LogError(thrown, "Something failed");

        string content = File.ReadAllText(provider.FilePath);
        content.Should().NotContain("Bronze Axe").And.NotContain("Bronze Ingot").And.NotContain("Tin Ore");
        content.Should().Contain("'***'");
        content.Should().Contain(nameof(InvalidOperationException), "the exception types stay in full");
    }

    [Test]
    public void Log_WithRedactor_CollapsesRootedPathsButLeavesOurOwnMessageWording()
    {
        string root = Path.Combine(Path.GetTempPath(), "cc_root");
        FileLoggerProvider provider = new(_logDirectory, new LogRedactor([root]));
        ILogger logger = provider.CreateLogger("test");

        // Our own messages are constants (rule L3), so only the path rule is applied to them.
        logger.LogError("Could not save 'the export' to {Path}", Path.Combine(root, "Exports", "Valheim.ccdata"));

        string content = File.ReadAllText(provider.FilePath);
        content.Should().NotContain("Valheim").And.NotContain(root);
        content.Should().Contain("<path>.ccdata");
        content.Should().Contain("'the export'");
    }

    [Test]
    public void Log_WithRedactor_LeavesNeitherTheRootNorTheFileNameInTheFile()
    {
        string root = Path.Combine(Path.GetTempPath(), "cc_root");
        FileLoggerProvider provider = new(_logDirectory, new LogRedactor([root]));
        ILogger logger = provider.CreateLogger("test");
        string exportPath = Path.Combine(root, "Exports", "Valheim-20260920-101500.ccdata");

        logger.LogError(new FileNotFoundException($"Could not find file '{exportPath}'."), "Export failed");

        string content = File.ReadAllText(provider.FilePath);
        content.Should().NotContain("Valheim").And.NotContain(root);
        content.Should().Contain(nameof(FileNotFoundException));
    }

    [Test]
    public void Log_PastSizeCap_RotatesTheCurrentFileToTheFirstArchive()
    {
        ILogger logger = _provider.CreateLogger("test");

        WritePastCap(logger, marker: "first");

        File.Exists(_provider.ArchivePath(1)).Should().BeTrue("the oversized file must rotate out");
        File.Exists(_provider.ArchivePath(2)).Should().BeFalse("one rotation fills only the first archive");
        new FileInfo(_provider.FilePath).Length.Should().BeLessThan(FileLoggerProvider.MaxFileSizeBytes,
            "a fresh current file starts after a rotation");
    }

    [Test]
    public void Log_SecondRotation_ShiftsTheFirstArchiveIntoTheSecond()
    {
        ILogger logger = _provider.CreateLogger("test");

        WritePastCap(logger, marker: "first");
        WritePastCap(logger, marker: "second");

        // "first-0" is the earliest entry there is, so it proves which file holds the oldest content. Later
        // "first" entries were still in the current file at the second rotation and legitimately move with it.
        File.ReadAllText(_provider.ArchivePath(2)).Should().Contain("first-0");
        File.ReadAllText(_provider.ArchivePath(1)).Should().Contain("second").And.NotContain("first-0");
    }

    [Test]
    public void Log_ThirdRotation_DropsTheOldestAndStaysBounded()
    {
        ILogger logger = _provider.CreateLogger("test");

        WritePastCap(logger, marker: "first");
        WritePastCap(logger, marker: "second");
        WritePastCap(logger, marker: "third");

        string[] files = Directory.GetFiles(_logDirectory);
        files.Should().HaveCount(FileLoggerProvider.RetainedArchives + 1,
            "the retained set is the app's entire log footprint, however long it runs");
        _provider.ReadAll().Should().NotContain("first-0", "the oldest archive is dropped, not kept");
        files.Sum(file => new FileInfo(file).Length).Should().BeLessThan(
            (FileLoggerProvider.RetainedArchives + 1) * (FileLoggerProvider.MaxFileSizeBytes + 2 * PaddingBytes),
            "each file rotates within one entry of the cap");
    }

    [Test]
    public void Log_WhenDirectoryIsUnwritable_DoesNotThrow()
    {
        // A file at the directory path makes Directory.CreateDirectory throw inside the provider.
        string fileBlockingDirectory = Path.Combine(Path.GetTempPath(), $"cc_blocked_{Guid.NewGuid():N}");
        File.WriteAllText(fileBlockingDirectory, "not a directory");

        try
        {
            FileLoggerProvider provider = new(fileBlockingDirectory, redactor: null);
            ILogger logger = provider.CreateLogger("test");

            Action logging = () => logger.LogInformation("goes nowhere");

            logging.Should().NotThrow("diagnostics must never themselves crash the app");
        }
        finally
        {
            File.Delete(fileBlockingDirectory);
        }
    }

    [Test]
    public void Log_ConcurrentWrites_AllEntriesLandIntact()
    {
        const int writers = 8;
        const int entriesPerWriter = 50;
        ILogger logger = _provider.CreateLogger("test");

        Task[] tasks =
        [
            .. Enumerable.Range(0, writers).Select(writer => Task.Run(() =>
            {
                for (int index = 0; index < entriesPerWriter; index++)
                {
                    logger.LogInformation("entry-marker w{Writer} i{Index}", writer, index);
                }
            }))
        ];
        Task.WaitAll(tasks);

        string content = File.ReadAllText(_provider.FilePath);
        (content.Split("entry-marker").Length - 1).Should().Be(writers * entriesPerWriter);
    }

    [Test]
    public void ReadAll_WithNothingLogged_ReturnsEmpty()
    {
        _provider.ReadAll().Should().BeEmpty();
    }

    [Test]
    public void ReadAll_ReturnsLoggedContent()
    {
        _provider.CreateLogger("test").LogInformation("only entry");

        _provider.ReadAll().Should().Contain("only entry");
    }

    [Test]
    public void ReadAll_AfterRotation_ReturnsTheArchivesOldestFirst()
    {
        ILogger logger = _provider.CreateLogger("test");
        WritePastCap(logger, marker: "first");
        WritePastCap(logger, marker: "second");
        logger.LogInformation("last entry");

        string all = _provider.ReadAll();

        // "first-0" only exists in the second archive and "last entry" only in the current file, so their
        // relative order proves the retained files are concatenated oldest first.
        all.IndexOf("first-0", StringComparison.Ordinal).Should().BeGreaterThan(-1);
        all.IndexOf("first-0", StringComparison.Ordinal)
            .Should().BeLessThan(all.IndexOf("last entry", StringComparison.Ordinal));
    }

    private static Exception CaptureThrownException()
    {
        try
        {
            throw new InvalidOperationException("the outer failure", new ArgumentException("the inner reason"));
        }
        catch (InvalidOperationException exception)
        {
            return exception;
        }
    }

    private static void WritePastCap(ILogger logger, string marker)
    {
        string padding = new('x', PaddingBytes);

        // The cap is checked before each append, so the file has to pass it and then be written to once more
        // for the rotation to happen.
        for (int index = 0; index <= FileLoggerProvider.MaxFileSizeBytes / PaddingBytes + 1; index++)
        {
            logger.LogInformation("{Marker} {Padding}", $"{marker}-{index}", padding);
        }
    }
}
