using System.Text.Json;
using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format.V1;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.Infrastructure.Files;

namespace CraftingCalculator.Infrastructure.UnitTests.Files;

[TestFixture]
public class ExportFileStoreTests
{
    private static readonly DateTimeOffset FirstExport = new(2026, 9, 12, 18, 4, 0, TimeSpan.Zero);

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

    private static TransferDocumentV1 Document(string datasetName, DateTimeOffset exportedAt) =>
        new(TransferFormat.Name, TransferFormat.CurrentVersion, exportedAt, "1.0", datasetName,
            [new CategoryV1(1, "Metals", "")], [], [], []);

    private static string ExpectedName(string stem, DateTimeOffset exportedAt) =>
        $"{stem}-{exportedAt.ToLocalTime():yyyyMMdd-HHmmss}{TransferFormat.FileExtension}";

    [Test]
    public async Task SaveAsync_NamesTheFileAfterTheDatasetAndExportTime()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("Valheim", FirstExport));

        saved.FileName.Should().Be(ExpectedName("Valheim", FirstExport));
        saved.FullPath.Should().Be(Path.Combine(_directory, saved.FileName));
        saved.DatasetName.Should().Be("Valheim");
        saved.CreatedAt.Should().Be(FirstExport);
    }

    [Test]
    public async Task SaveAsync_StripsCharactersAFileNameCannotHold()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("Rust: \"Modded\" / PvE?.", FirstExport));

        saved.FileName.Should().Be(ExpectedName("Rust Modded  PvE", FirstExport));
        saved.DatasetName.Should().Be("Rust: \"Modded\" / PvE?.");
    }

    [Test]
    public async Task SaveAsync_ANameOfNothingButReservedCharacters_StillSaves()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("???", FirstExport));

        saved.FileName.Should().Be(ExpectedName("Dataset", FirstExport));
    }

    [Test]
    public async Task SaveAsync_WritesTheDocumentAndNoTempFile()
    {
        TransferDocumentV1 document = Document("Valheim", FirstExport);

        ExportFileInfo saved = await _store.SaveAsync(document);

        Directory.GetFiles(_directory).Should().Equal(saved.FullPath);

        await using FileStream stream = File.OpenRead(saved.FullPath);
        (await JsonSerializer.DeserializeAsync(stream, TransferJsonContext.Default.TransferDocumentV1))
            .Should().BeEquivalentTo(document);
    }

    [Test]
    public async Task SaveAsync_RemovesATempFileLeftByAnInterruptedExport()
    {
        Directory.CreateDirectory(_directory);
        string leftover = Path.Combine(_directory, "Valheim-20260911-120000.ccdata.tmp");
        await File.WriteAllTextAsync(leftover, "{ \"format\": ");

        await _store.SaveAsync(Document("Valheim", FirstExport));

        File.Exists(leftover).Should().BeFalse();
    }

    [Test]
    public async Task SaveAsync_TwoExportsInTheSameSecond_KeepsBoth()
    {
        ExportFileInfo first = await _store.SaveAsync(Document("Valheim", FirstExport));
        ExportFileInfo second = await _store.SaveAsync(Document("Valheim", FirstExport));

        second.FullPath.Should().NotBe(first.FullPath);
        File.Exists(first.FullPath).Should().BeTrue();
        File.Exists(second.FullPath).Should().BeTrue();
    }

    [Test]
    public async Task SaveAsync_ASixthExport_DeletesTheOldest()
    {
        List<ExportFileInfo> saved = [];

        for (int export = 0; export < 6; export++)
        {
            saved.Add(await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(export))));
        }

        Directory.GetFiles(_directory).Should().HaveCount(5);
        File.Exists(saved[0].FullPath).Should().BeFalse();
        File.Exists(saved[5].FullPath).Should().BeTrue();
    }

    [Test]
    public async Task SaveAsync_LeavesFilesThatAreNotExportsAlone()
    {
        Directory.CreateDirectory(_directory);
        string notes = Path.Combine(_directory, "notes.txt");
        await File.WriteAllTextAsync(notes, "Karve needs 10 nails");

        for (int export = 0; export < 6; export++)
        {
            await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(export)));
        }

        File.Exists(notes).Should().BeTrue();
    }

    [Test]
    public async Task SaveAsync_AFileThatIsNotAnExport_DoesNotTakeTheSlotOfARealOne()
    {
        Directory.CreateDirectory(_directory);
        string impostor = Path.Combine(_directory, $"Renamed{TransferFormat.FileExtension}");
        await File.WriteAllTextAsync(impostor, "definitely not json");
        File.SetLastWriteTimeUtc(impostor, DateTime.UtcNow.AddHours(1));
        List<ExportFileInfo> saved = [];

        for (int export = 0; export < 6; export++)
        {
            saved.Add(await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(export))));
        }

        File.Exists(impostor).Should().BeTrue();
        File.Exists(saved[0].FullPath).Should().BeFalse();
        saved.Skip(1).Should().OnlyContain(export => File.Exists(export.FullPath));
    }

    [Test]
    public void GetLatest_NoFolderYet_ReturnsNull() => _store.GetLatest().Should().BeNull();

    [Test]
    public async Task GetLatest_ReturnsTheNewestExport()
    {
        await _store.SaveAsync(Document("Valheim", FirstExport));
        ExportFileInfo newest = await _store.SaveAsync(Document("Rust", FirstExport.AddMinutes(1)));

        _store.GetLatest().Should().Be(newest with { FullPath = new FileInfo(newest.FullPath).FullName });
    }

    [Test]
    public async Task GetLatest_AfterTheUserDeletesEveryExport_ReturnsNull()
    {
        ExportFileInfo saved = await _store.SaveAsync(Document("Valheim", FirstExport));

        File.Delete(saved.FullPath);

        _store.GetLatest().Should().BeNull();
    }

    [Test]
    public async Task GetLatest_AfterTheUserDeletesTheNewest_ReturnsTheOneBefore()
    {
        ExportFileInfo older = await _store.SaveAsync(Document("Valheim", FirstExport));
        ExportFileInfo newest = await _store.SaveAsync(Document("Valheim", FirstExport.AddMinutes(1)));

        File.Delete(newest.FullPath);

        _store.GetLatest()!.FileName.Should().Be(older.FileName);
    }

    [Test]
    public async Task GetLatest_SkipsAFileThatIsNotAnExport()
    {
        ExportFileInfo export = await _store.SaveAsync(Document("Valheim", FirstExport));
        string impostor = Path.Combine(_directory, $"Renamed{TransferFormat.FileExtension}");
        await File.WriteAllTextAsync(impostor, "definitely not json");
        File.SetLastWriteTimeUtc(impostor, DateTime.UtcNow.AddHours(1));

        _store.GetLatest()!.FileName.Should().Be(export.FileName);
    }
}
