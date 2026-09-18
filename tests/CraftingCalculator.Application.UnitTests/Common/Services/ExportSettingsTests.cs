using AwesomeAssertions;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Constants;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class ExportSettingsTests
{
    private FakePreferenceStore _preferences = null!;

    [SetUp]
    public void SetUp() => _preferences = new FakePreferenceStore();

    [Test]
    public void KeptExports_WithNothingStored_IsTheDefault() =>
        new ExportSettings(_preferences).KeptExports.Should().Be(ExportConstants.DefaultKeptExports);

    [Test]
    public void KeptExports_WithSomethingThatIsNotANumberStored_IsTheDefault()
    {
        _preferences.Set("kept_exports", "loads");

        new ExportSettings(_preferences).KeptExports.Should().Be(ExportConstants.DefaultKeptExports);
    }

    [Test]
    public void SetKeptExports_IsReadBackByTheNextLaunch()
    {
        new ExportSettings(_preferences).SetKeptExports(9);

        new ExportSettings(_preferences).KeptExports.Should().Be(9);
    }

    [TestCase(0)]
    [TestCase(-3)]
    public void SetKeptExports_BelowTheMinimum_ClampsToIt(int count)
    {
        ExportSettings settings = new(_preferences);

        settings.SetKeptExports(count);

        settings.KeptExports.Should().Be(ExportConstants.MinimumKeptExports);
    }

    [Test]
    public void SetKeptExports_AboveTheMaximum_ClampsToIt()
    {
        ExportSettings settings = new(_preferences);

        settings.SetKeptExports(ExportConstants.MaximumKeptExports + 1);

        settings.KeptExports.Should().Be(ExportConstants.MaximumKeptExports);
    }

    [Test]
    public void KeptExports_AStoredValueOutsideTheRange_ClampsToIt()
    {
        _preferences.Set("kept_exports", (ExportConstants.MaximumKeptExports + 100).ToString());

        new ExportSettings(_preferences).KeptExports.Should().Be(ExportConstants.MaximumKeptExports);
    }
}
