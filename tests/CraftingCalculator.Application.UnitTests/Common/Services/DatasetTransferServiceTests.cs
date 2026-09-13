using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format.V1;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;
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
    private DatasetTransferService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _datasetDAO = new Mock<IDatasetDAO>();
        _selectedDataset = new Mock<ISelectedDatasetState>();
        _exportFileStore = new Mock<IExportFileStore>();
        _service = new DatasetTransferService(
            _datasetDAO.Object, _selectedDataset.Object, _exportFileStore.Object, new FixedTimeProvider(Now));
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
        TransferDocumentV1? written = null;
        _exportFileStore.Setup(store => store.SaveAsync(It.IsAny<TransferDocumentV1>()))
            .Callback<TransferDocumentV1>(document => written = document)
            .ReturnsAsync(saved);

        ExportFileInfo result = await _service.ExportAsync(BronzeChain.Snapshot, selected, "1.2");

        result.Should().BeSameAs(saved);
        written!.ExportedAt.Should().Be(Now);
        written.AppVersion.Should().Be("1.2");
        written.Components.Select(component => component.Name).Should().Equal("Copper");
    }

    [Test]
    public async Task ExportAsync_ASelectionMissingADependency_ThrowsWithoutSaving()
    {
        HashSet<RecordKey> selected = [BronzeChain.Copper];

        Func<Task> act = () => _service.ExportAsync(BronzeChain.Snapshot, selected, "1.0");

        await act.Should().ThrowAsync<InvalidOperationException>();
        _exportFileStore.Verify(store => store.SaveAsync(It.IsAny<TransferDocumentV1>()), Times.Never);
    }

    [Test]
    public void GetLatestExport_ReturnsTheStoresLatestFile()
    {
        ExportFileInfo latest = new("/exports/Valheim.ccdata", "Valheim.ccdata", "Valheim", Now);
        _exportFileStore.Setup(store => store.GetLatest()).Returns(latest);

        _service.GetLatestExport().Should().BeSameAs(latest);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
