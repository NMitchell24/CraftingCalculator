using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Services.Impl;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class DiagnosticSettingsTests
{
    private FakePreferenceStore _preferences = null!;
    private Mock<IDiagnosticLog> _log = null!;

    [SetUp]
    public void SetUp()
    {
        _preferences = new FakePreferenceStore();
        _log = new Mock<IDiagnosticLog>();
        _log.SetupProperty(log => log.TraceEnabled);
    }

    [Test]
    public void TraceLogging_WithNothingStored_IsOffAndTheLogIsToldSo()
    {
        _log.Object.TraceEnabled = true;

        new DiagnosticSettings(_preferences, _log.Object).TraceLogging.Should().BeFalse();

        _log.Object.TraceEnabled.Should().BeFalse();
    }

    [Test]
    public void TraceLogging_WithSomethingThatIsNotABoolStored_IsOff()
    {
        _preferences.Set("trace_logging", "loads");

        new DiagnosticSettings(_preferences, _log.Object).TraceLogging.Should().BeFalse();
    }

    [Test]
    public void TraceLogging_StoredOn_IsAppliedToTheLogOnConstruction()
    {
        _preferences.Set("trace_logging", bool.TrueString);

        _ = new DiagnosticSettings(_preferences, _log.Object);

        _log.Object.TraceEnabled.Should().BeTrue();
    }

    [Test]
    public void SetTraceLogging_AppliesToTheLogAtOnce()
    {
        DiagnosticSettings settings = new(_preferences, _log.Object);

        settings.SetTraceLogging(true);

        _log.Object.TraceEnabled.Should().BeTrue();
        settings.TraceLogging.Should().BeTrue();
    }

    [Test]
    public void SetTraceLogging_IsReadBackByTheNextLaunch()
    {
        new DiagnosticSettings(_preferences, _log.Object).SetTraceLogging(true);

        Mock<IDiagnosticLog> nextLaunch = new();
        nextLaunch.SetupProperty(log => log.TraceEnabled);
        new DiagnosticSettings(_preferences, nextLaunch.Object).TraceLogging.Should().BeTrue();
    }

    [Test]
    public void SetTraceLogging_WhenTheStoreThrows_LeavesTheLogAsItWas()
    {
        Mock<IPreferenceStore> failing = new();
        failing.Setup(store => store.Set(It.IsAny<string>(), It.IsAny<string>())).Throws<IOException>();
        DiagnosticSettings settings = new(failing.Object, _log.Object);

        Action act = () => settings.SetTraceLogging(true);

        act.Should().Throw<IOException>();
        _log.Object.TraceEnabled.Should().BeFalse();
    }
}
