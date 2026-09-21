using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.Infrastructure.UnitTests.Logging;

/// <summary>
/// The filters <see cref="DependencyInjection.AddFileLogging"/> registers, exercised through a real
/// <see cref="ILoggerFactory"/> so the framework's own level rules are part of what is tested.
/// </summary>
[TestFixture]
public class FileLoggingRegistrationTests
{
    private string _logDirectory = null!;
    private ServiceProvider _services = null!;
    private IDiagnosticLog _log = null!;

    [SetUp]
    public void Setup()
    {
        _logDirectory = Path.Combine(Path.GetTempPath(), $"cc_logs_{Guid.NewGuid():N}");
        _services = new ServiceCollection().AddLogging(logging => logging.AddFileLogging(_logDirectory))
            .BuildServiceProvider();
        _log = _services.GetRequiredService<IDiagnosticLog>();
    }

    [TearDown]
    public void TearDown()
    {
        _services.Dispose();

        if (Directory.Exists(_logDirectory))
        {
            Directory.Delete(_logDirectory, recursive: true);
        }
    }

    // The factory belongs to the container and is disposed with it.
    private ILogger Logger(string category) => _services.GetRequiredService<ILoggerFactory>().CreateLogger(category);

    [Test]
    public void AppCategory_WithTraceOff_WritesInformationButNotTrace()
    {
        ILogger logger = Logger("CraftingCalculator.UI.State.ActionGuard");

        logger.LogTrace("a breadcrumb");
        logger.LogInformation("a session header");

        string content = _log.ReadAll();
        content.Should().NotContain("a breadcrumb");
        content.Should().Contain("a session header");
    }

    [Test]
    public void AppCategory_WithTraceOn_WritesTrace()
    {
        _log.TraceEnabled = true;

        Logger("CraftingCalculator.UI.State.ActionGuard").LogTrace("a breadcrumb");

        _log.ReadAll().Should().Contain("a breadcrumb");
    }

    [Test]
    public void OtherCategories_WithTraceOn_KeepTheirOwnFloors()
    {
        _log.TraceEnabled = true;

        Logger("MudBlazor.KeyInterceptorService").LogTrace("library chatter");
        Logger("MudBlazor.KeyInterceptorService").LogInformation("library information");
        Logger("Microsoft.AspNetCore.Components.RenderTree").LogInformation("framework chatter");
        Logger("Microsoft.AspNetCore.Components.RenderTree").LogWarning("framework warning");
        Logger("Microsoft.EntityFrameworkCore.Database.Command").LogDebug("an EF command");
        Logger("Microsoft.EntityFrameworkCore.Database.Command").LogError("an EF failure");

        string content = _log.ReadAll();
        content.Should().NotContain("library chatter", "only the app's own categories are opened to Trace");
        content.Should().Contain("library information");
        content.Should().NotContain("framework chatter");
        content.Should().Contain("framework warning");
        content.Should().NotContain("an EF command");
        content.Should().NotContain("an EF failure", "EF can log the user's data as parameter values");
    }
}
