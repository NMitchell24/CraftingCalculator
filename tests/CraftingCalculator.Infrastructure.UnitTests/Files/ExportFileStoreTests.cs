using System.Text.Json;
using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.Infrastructure.Files;

namespace CraftingCalculator.Infrastructure.UnitTests.Files;

[TestFixture]
public class ExportFileStoreTests
{
    private static readonly DateTimeOffset FirstExport = new(2026, 9, 12, 18, 4, 0, TimeSpan.Zero);

    /// <summary>How many exports the tests that are not about retention ask the store to keep.</summary>
    private const int Keep = 5;

    private string _directory = null!;
    private ExportFileStore _store = null!;

    [SetUp]
    public void SetUp()
    {
        _directory = Path.Combine(Path.GetTempPath(), $"crafting_exports_{Guid.NewGuid():N}");
        _store = new ExportFileStore(_directory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private static TransferDocument Document(string datasetName, DateTimeOffset exportedAt) =>
        new(TransferFormat.Name, TransferFormat.CurrentVersion, exportedAt, "1.0", datasetName,
            [new TransferCategory(1, "Metals", "")], [], [], []);

    private static string ExpectedName(string stem, DateTimeOffset exportedAt) =>
        $"{stem}-{exportedAt.ToLocalTime():yyyyMMdd-HHmmss}{TransferFormat.FileExtension}";

    [Test]
    public async Task SaveAsync_NamesTheFileAfterTheDatasetAndExportTime()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);

        saved.FileName.Should().Be(ExpectedName("Valheim", FirstExport));
        saved.FullPath.Should().Be(Path.Combine(_directory, saved.FileName));
        saved.DatasetName.Should().Be("Valheim");
        saved.CreatedAt.Should().Be(FirstExport);
    }

    [Test]
    public async Task SaveAsync_StripsCharactersAFileNameCannotHold()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("Rust: \"Modded\" / PvE?.", FirstExport), Keep);

        saved.FileName.Should().Be(ExpectedName("Rust Modded  PvE", FirstExport));
        saved.DatasetName.Should().Be("Rust: \"Modded\" / PvE?.");
    }

    [Test]
    public async Task SaveAsync_ANameOfNothingButReservedCharacters_StillSaves()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("???", FirstExport), Keep);

        saved.FileName.Should().Be(ExpectedName("Dataset", FirstExport));
    }

    [Test]
    public async Task SaveAsync_WritesTheDocumentAndNoTempFile()
    {
        TransferDocument document = Document("Valheim", FirstExport);

        ExportFileInfo saved = await _store.SaveAsync(document, Keep);

        Directory.GetFiles(_directory).Should().Equal(saved.FullPath);

        await using FileStream stream = File.OpenRead(saved.FullPath);
        (await JsonSerializer.DeserializeAsync(stream, TransferJsonContext.Default.TransferDocument))
            .Should().BeEquivalentTo(document);
    }

    [Test]
    public async Task SaveAsync_RemovesATempFileLeftByAnInterruptedExport()
    {
        Directory.CreateDirectory(_directory);
        string leftover = Path.Combine(_directory, "Valheim-20260911-120000.ccdata.tmp");
        await File.WriteAllTextAsync(leftover, "{ \"format\": ");

        await _store.SaveAsync(Document("Valheim", FirstExport), Keep);

        File.Exists(leftover).Should().BeFalse();
    }

    [Test]
    public async Task SaveAsync_TwoExportsInTheSameSecond_KeepsBoth()
    {
        ExportFileInfo first = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);
        ExportFileInfo second = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);

        second.FullPath.Should().NotBe(first.FullPath);
        File.Exists(first.FullPath).Should().BeTrue();
        File.Exists(second.FullPath).Should().BeTrue();
    }

    [Test]
    public async Task SaveAsync_OneExportPastTheKeepCount_DeletesTheOldest()
    {
        List<ExportFileInfo> saved = [];

        for (int export = 0; export < Keep + 1; export++)
        {
            saved.Add(await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(export)), Keep));
        }

        Directory.GetFiles(_directory).Should().HaveCount(Keep);
        File.Exists(saved[0].FullPath).Should().BeFalse();
        File.Exists(saved[^1].FullPath).Should().BeTrue();
    }

    [Test]
    public async Task SaveAsync_KeepingOne_LeavesOnlyTheNewExport()
    {
        ExportFileInfo first = await _store.SaveAsync(Document("Valheim", FirstExport), keep: 1);

        ExportFileInfo second = await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(1)), keep: 1);

        Directory.GetFiles(_directory).Should().Equal(second.FullPath);
        File.Exists(first.FullPath).Should().BeFalse();
    }

    [Test]
    public async Task SaveAsync_KeepingOne_AfterTheClockGoesBack_KeepsTheNewExport()
    {
        ExportFileInfo beforeClockChange = await _store.SaveAsync(Document("Valheim", FirstExport.AddHours(1)), keep: 1);
        File.SetLastWriteTimeUtc(beforeClockChange.FullPath, DateTime.UtcNow.AddHours(1));

        ExportFileInfo afterClockChange = await _store.SaveAsync(Document("Valheim", FirstExport), keep: 1);

        File.Exists(afterClockChange.FullPath).Should().BeTrue();
        File.Exists(beforeClockChange.FullPath).Should().BeFalse();
    }

    [Test]
    public async Task SaveAsync_AfterTheKeepCountDrops_DeletesEveryExportPastTheNewCount()
    {
        List<ExportFileInfo> saved = [];

        for (int export = 0; export < 5; export++)
        {
            saved.Add(await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(export)), keep: 5));
        }

        ExportFileInfo newest = await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(5)), keep: 2);

        Directory.GetFiles(_directory).Should().HaveCount(2);
        File.Exists(newest.FullPath).Should().BeTrue();
        File.Exists(saved[^1].FullPath).Should().BeTrue();
    }

    [Test]
    public async Task SaveAsync_LeavesFilesThatAreNotExportsAlone()
    {
        Directory.CreateDirectory(_directory);
        string notes = Path.Combine(_directory, "notes.txt");
        await File.WriteAllTextAsync(notes, "Karve needs 10 nails");

        for (int export = 0; export < 6; export++)
        {
            await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(export)), Keep);
        }

        File.Exists(notes).Should().BeTrue();
    }

    [Test]
    public async Task SaveAsync_AFileThatIsNotAnExport_DoesNotTakeTheSlotOfARealOne()
    {
        Directory.CreateDirectory(_directory);
        string impostor = Path.Combine(_directory, $"Renamed{TransferFormat.FileExtension}");
        await File.WriteAllTextAsync(impostor, "definitely not json");
        List<ExportFileInfo> saved = [];

        for (int export = 0; export < 6; export++)
        {
            saved.Add(await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(export)), Keep));
        }

        File.Exists(impostor).Should().BeTrue();
        File.Exists(saved[0].FullPath).Should().BeFalse();
        saved.Skip(1).Should().OnlyContain(export => File.Exists(export.FullPath));
    }

    [Test]
    public void List_NoFolderYet_ReturnsNothing() => _store.List().Should().BeEmpty();

    [Test]
    public async Task List_ReturnsEveryExportNewestFirst()
    {
        ExportFileInfo older = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);
        ExportFileInfo newest = await _store.SaveAsync(Document("Rust", FirstExport.AddMinutes(1)), Keep);

        _store.List().Select(export => export.FileName).Should().Equal(newest.FileName, older.FileName);
    }

    [Test]
    public async Task List_OrdersByExportTime_WhateverTheFileWriteTimes()
    {
        ExportFileInfo older = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);
        ExportFileInfo newest = await _store.SaveAsync(Document("Rust", FirstExport.AddMinutes(1)), Keep);
        File.SetLastWriteTimeUtc(newest.FullPath, DateTime.UtcNow.AddHours(-1));

        _store.List().Select(export => export.FileName).Should().Equal(newest.FileName, older.FileName);
    }

    [Test]
    public async Task List_ReadsTheDatasetNameAndTimeOutOfEachFile()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);

        _store.List().Should().Equal(saved with { FullPath = new FileInfo(saved.FullPath).FullName });
    }

    [Test]
    public async Task List_AfterTheUserDeletesEveryExport_ReturnsNothing()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);

        File.Delete(saved.FullPath);

        _store.List().Should().BeEmpty();
    }

    [Test]
    public async Task List_SkipsAFileThatIsNotAnExport()
    {
        ExportFileInfo export = await _store.SaveAsync(Document("Valheim", FirstExport), Keep);
        string impostor = Path.Combine(_directory, $"Renamed{TransferFormat.FileExtension}");
        await File.WriteAllTextAsync(impostor, "definitely not json");

        _store.List().Select(file => file.FileName).Should().Equal(export.FileName);
    }
}
