using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class DatasetTransferServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 12, 18, 4, 0, TimeSpan.Zero);

    private Mock<IDatasetDAO> _datasetDAO = null!;
    private Mock<ISelectedDatasetState> _selectedDataset = null!;
    private Mock<IExportFileStore> _exportFileStore = null!;
    private Mock<IExportSettings> _exportSettings = null!;
    private DatasetTransferService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _datasetDAO = new Mock<IDatasetDAO>();
        _selectedDataset = new Mock<ISelectedDatasetState>();
        _exportFileStore = new Mock<IExportFileStore>();
        _exportSettings = new Mock<IExportSettings>();
        _service = new DatasetTransferService(
            _datasetDAO.Object, _selectedDataset.Object, _exportFileStore.Object, _exportSettings.Object,
            new FixedTimeProvider(Now));
    }

    [Test]
    public async Task LoadCurrentSnapshotAsync_ReadsTheSelectedDataset()
    {
        _selectedDataset.SetupGet(state => state.Id).Returns(7);
        _datasetDAO.Setup(dao => dao.GetSnapshotAsync(7)).ReturnsAsync(BronzeChain.Snapshot);

        (await _service.LoadCurrentSnapshotAsync()).Should().BeSameAs(BronzeChain.Snapshot);
    }

    [Test]
    public async Task ExportAsync_SavesTheSelectionStampedWithTheTimeAndVersion()
    {
        HashSet<RecordKey> selected = [BronzeChain.Metals, BronzeChain.Copper];
        ExportFileInfo saved = new("/exports/Valheim.ccdata", "Valheim.ccdata", "Valheim", Now);
        TransferDocument? written = null;
        _exportFileStore.Setup(store => store.SaveAsync(It.IsAny<TransferDocument>(), It.IsAny<int>()))
            .Callback<TransferDocument, int>((document, _) => written = document)
            .ReturnsAsync(saved);

        ExportFileInfo result = await _service.ExportAsync(BronzeChain.Snapshot, selected, "1.2");

        result.Should().BeSameAs(saved);
        written!.ExportedAt.Should().Be(Now);
        written.AppVersion.Should().Be("1.2");
        written.Components.Select(component => component.Name).Should().Equal("Copper");
    }

    [Test]
    public async Task ExportAsync_KeepsAsManyExportsAsTheSettingSays()
    {
        HashSet<RecordKey> selected = [BronzeChain.Metals, BronzeChain.Copper];
        _exportSettings.SetupGet(settings => settings.KeptExports).Returns(3);
        _exportFileStore.Setup(store => store.SaveAsync(It.IsAny<TransferDocument>(), It.IsAny<int>()))
            .ReturnsAsync(new ExportFileInfo("/exports/Valheim.ccdata", "Valheim.ccdata", "Valheim", Now));

        await _service.ExportAsync(BronzeChain.Snapshot, selected, "1.2");

        _exportFileStore.Verify(store => store.SaveAsync(It.IsAny<TransferDocument>(), 3), Times.Once);
    }

    [Test]
    public async Task ExportAsync_ASelectionMissingADependency_ThrowsWithoutSaving()
    {
        HashSet<RecordKey> selected = [BronzeChain.Copper];

        Func<Task> act = () => _service.ExportAsync(BronzeChain.Snapshot, selected, "1.0");

        await act.Should().ThrowAsync<InvalidOperationException>();
        _exportFileStore.Verify(store => store.SaveAsync(It.IsAny<TransferDocument>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void ListExports_ReturnsTheStoresFiles()
    {
        ExportFileInfo[] exports = [new("/exports/Valheim.ccdata", "Valheim.ccdata", "Valheim", Now)];
        _exportFileStore.Setup(store => store.List()).Returns(exports);

        _service.ListExports().Should().BeSameAs(exports);
    }

    [Test]
    public async Task ImportAsNewAsync_AddsADatasetWithTheTrimmedName()
    {
        DatasetModel created = new() { Id = 3, Name = "Valheim" };
        _datasetDAO.Setup(dao => dao.ImportAsNewAsync("Valheim", BronzeChain.Snapshot)).ReturnsAsync(created);

        (await _service.ImportAsNewAsync(BronzeChain.Snapshot, "  Valheim ")).Should().BeSameAs(created);
    }

    [Test]
    public async Task FindConflictsAsync_ComparesWithTheNamedDataset()
    {
        DatasetSnapshot incoming = BronzeChain.Snapshot with { Categories = [], Components = [], Blueprints = [], Favorites = [BronzeChain.Snapshot.Favorites[0] with { Blueprints = [] }] };
        _datasetDAO.Setup(dao => dao.GetSnapshotAsync(4)).ReturnsAsync(BronzeChain.Snapshot);

        IReadOnlyList<ImportConflict> conflicts = await _service.FindConflictsAsync(4, incoming);

        conflicts.Should().Equal(new ImportConflict(RecordKind.Favorite, 1, 1, "Bronze Axe run"));
    }

    [Test]
    public async Task MergeAsync_WithNoLoop_WritesThePlanWithTheConflictsFoundNow()
    {
        HashSet<RecordKey> replace = [BronzeChain.Copper];
        MergePlan? written = null;
        _datasetDAO.Setup(dao => dao.GetSnapshotAsync(4)).ReturnsAsync(BronzeChain.Snapshot);
        _datasetDAO.Setup(dao => dao.MergeAsync(4, It.IsAny<MergePlan>()))
            .Callback<int, MergePlan>((_, plan) => written = plan)
            .Returns(Task.CompletedTask);

        IReadOnlyList<string> cycles = await _service.MergeAsync(4, BronzeChain.Snapshot, replace);

        cycles.Should().BeEmpty();
        written!.Incoming.Should().BeSameAs(BronzeChain.Snapshot);
        written.Conflicts.Should().HaveCount(11);
        written.Replace.Should().BeSameAs(replace);
    }

    [Test]
    public async Task MergeAsync_ThatWouldNestABlueprintInsideItself_ReturnsItsNamesWithoutWriting()
    {
        // Mine: the Bronze Axe nests Bronze. The file: Bronze nests the Bronze Axe. Keeping my Axe and replacing
        // Bronze makes a loop.
        DatasetSnapshot incoming = BronzeChain.Snapshot with
        {
            Blueprints =
            [
                BronzeChain.Snapshot.Blueprints[0] with { Blueprints = [new QuantityLink(BronzeChain.BronzeAxe.Id, 1)] },
                BronzeChain.Snapshot.Blueprints[1] with { Blueprints = [] }
            ],
            Favorites = []
        };
        _datasetDAO.Setup(dao => dao.GetSnapshotAsync(4)).ReturnsAsync(BronzeChain.Snapshot);

        IReadOnlyList<string> cycles = await _service.MergeAsync(4, incoming, new HashSet<RecordKey> { BronzeChain.Bronze });

        cycles.Should().Equal("Bronze", "Bronze Axe");
        _datasetDAO.Verify(dao => dao.MergeAsync(It.IsAny<int>(), It.IsAny<MergePlan>()), Times.Never);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
