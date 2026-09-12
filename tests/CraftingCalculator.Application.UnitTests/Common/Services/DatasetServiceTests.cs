using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Models;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class DatasetServiceTests
{
    private const string SelectedDatasetKey = "selectedDatasetId";

    private Mock<IDatasetDAO> _dao = null!;
    private FakePreferenceStore _preferences = null!;
    private SelectedDatasetState _selected = null!;
    private DatasetService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _dao = new Mock<IDatasetDAO>();
        _preferences = new FakePreferenceStore();
    }

    /// <summary>
    /// Builds the service over a selection state that has already read <paramref name="storedId"/> back
    /// from the preference store, which is the state a fresh launch starts in.
    /// </summary>
    private void GivenStoredSelection(int? storedId)
    {
        if (storedId is int id)
        {
            _preferences.Set(SelectedDatasetKey, id.ToString());
        }

        _selected = new SelectedDatasetState(_preferences);
        _service = new DatasetService(_dao.Object, _selected);
    }

    private void GivenDatasets(params DatasetModel[] datasets)
    {
        _dao.Setup(dao => dao.GetAllAsync()).ReturnsAsync([.. datasets]);
        _dao.Setup(dao => dao.CountAsync()).ReturnsAsync(datasets.Length);

        foreach (DatasetModel dataset in datasets)
        {
            _dao.Setup(dao => dao.GetByIdAsync(dataset.Id)).ReturnsAsync(dataset);
        }
    }

    [Test]
    public async Task InitializeAsync_StoredDatasetStillExists_KeepsIt()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(7);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(7);
    }

    [Test]
    public async Task InitializeAsync_NothingStored_SelectsTheFirstDatasetAndPersistsIt()
    {
        GivenDatasets(new DatasetModel { Id = 3, Name = "Ark" }, new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(null);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(3);

        // Written back, so the fallback is resolved once rather than on every launch.
        _preferences.Get(SelectedDatasetKey).Should().Be("3");
    }

    [Test]
    public async Task InitializeAsync_StoredDatasetIsGone_FallsBackToTheFirstDataset()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(99);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(1);
    }

    [Test]
    public async Task InitializeAsync_NoDatasetsAtAll_CreatesDefault()
    {
        GivenDatasets();
        _dao.Setup(dao => dao.AddAsync("Default")).ReturnsAsync(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(null);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(1);
        _dao.Verify(dao => dao.AddAsync("Default"), Times.Once);
    }

    [Test]
    public async Task SwitchToAsync_PersistsTheNewSelection()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(1);

        await _service.SwitchToAsync(7);

        _selected.Id.Should().Be(7);
        _preferences.Get(SelectedDatasetKey).Should().Be("7");
    }

    [Test]
    public async Task SwitchToAsync_DatasetNoLongerExists_LeavesTheSelectionAlone()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(1);

        await _service.SwitchToAsync(99);

        _selected.Id.Should().Be(1);
    }

    [Test]
    public async Task DeleteAsync_TheLastDataset_DeletesNothing()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(1);

        await _service.DeleteAsync(1);

        _dao.Verify(dao => dao.DeleteAsync(It.IsAny<int>()), Times.Never);
        _selected.Id.Should().Be(1);
    }

    [Test]
    public async Task DeleteAsync_TheSelectedDataset_MovesTheSelectionFirst()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(7);

        await _service.DeleteAsync(7);

        // Moved before the delete: the cascade runs against a dataset nothing is scoped to any more.
        _selected.Id.Should().Be(1);
        _preferences.Get(SelectedDatasetKey).Should().Be("1");
        _dao.Verify(dao => dao.DeleteAsync(7), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ADatasetThatIsNotSelected_LeavesTheSelectionAlone()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(1);

        await _service.DeleteAsync(7);

        _selected.Id.Should().Be(1);
        _dao.Verify(dao => dao.DeleteAsync(7), Times.Once);
    }

    [Test]
    public async Task NameExistsAsync_TrimsBeforeComparing()
    {
        GivenStoredSelection(1);
        _dao.Setup(dao => dao.GetByNameAsync("Rust", 0)).ReturnsAsync(new DatasetModel { Id = 7, Name = "Rust" });

        (await _service.NameExistsAsync("  Rust  ")).Should().BeTrue();
    }

    [Test]
    public async Task CreateAndRename_TrimTheName()
    {
        GivenStoredSelection(1);
        _dao.Setup(dao => dao.AddAsync("Rust")).ReturnsAsync(new DatasetModel { Id = 7, Name = "Rust" });

        await _service.CreateAsync("  Rust  ");
        await _service.RenameAsync(7, "  Valheim  ");

        _dao.Verify(dao => dao.AddAsync("Rust"), Times.Once);
        _dao.Verify(dao => dao.RenameAsync(7, "Valheim"), Times.Once);
    }

    [Test]
    public async Task CopyAsync_TrimsTheNameAndCopiesTheDatasetItIsGiven()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(1);
        _dao.Setup(dao => dao.CopyAsync(7, "Rust - Modded")).ReturnsAsync(new DatasetModel { Id = 9, Name = "Rust - Modded" });

        DatasetModel copy = await _service.CopyAsync(7, "  Rust - Modded  ");

        copy.Id.Should().Be(9);

        // The selection is the caller's to move: copying a dataset other than the selected one is a
        // supported call, and it does not make that one the one you are working in.
        _selected.Id.Should().Be(1);
    }

    /// <summary>Stands in for MAUI Preferences, which the Application layer never sees directly.</summary>
    private sealed class FakePreferenceStore : IPreferenceStore
    {
        private readonly Dictionary<string, string> _values = [];

        public string? Get(string key) => _values.GetValueOrDefault(key);

        public void Set(string key, string value) => _values[key] = value;
    }
}
