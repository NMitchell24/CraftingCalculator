using AwesomeAssertions;
using CraftingCalculator.Infrastructure.Logging;
using Microsoft.Data.Sqlite;

namespace CraftingCalculator.Infrastructure.UnitTests.Logging;

[TestFixture]
public class LogRedactorTests
{
    // The two shapes the app actually produces: an unpackaged Windows install, whose path carries the
    // account name, and Android's per-user data directory.
    private const string WindowsRoot = @"C:\Users\UserName\AppData\Local\CraftingCalculator";
    private const string AndroidRoot = "/data/user/0/com.sterlingtp.craftingcalculator/files";

    private static readonly LogRedactor Redactor = new([WindowsRoot, AndroidRoot]);

    [Test]
    public void CollapsePaths_RootedFile_KeepsOnlyTheExtension()
    {
        string collapsed = Redactor.CollapsePaths($@"Access to the path {WindowsRoot}\Exports\Valheim.ccdata is denied");

        collapsed.Should().Be("Access to the path <path>.ccdata is denied");
    }

    [Test]
    public void CollapsePaths_UnquotedJavaStylePath_StopsAtTheColon()
    {
        string collapsed = Redactor.CollapsePaths($"{AndroidRoot}/Exports/Valheim-20260920.ccdata: open failed");

        collapsed.Should().Be("<path>.ccdata: open failed");
    }

    [Test]
    public void CollapsePaths_RootedFolder_CollapsesWithNoExtension()
    {
        // The package name's dots sit in a directory segment and must not be read as an extension.
        string collapsed = Redactor.CollapsePaths($"{AndroidRoot}/Exports could not be created");

        collapsed.Should().Be("<path> could not be created");
    }

    [Test]
    public void CollapsePaths_WindowsRoot_TakesTheAccountNameWithIt()
    {
        string collapsed = Redactor.CollapsePaths($@"Could not find a part of the path {WindowsRoot}\Logs");

        collapsed.Should().NotContain("UserName").And.Be("Could not find a part of the path <path>");
    }

    [Test]
    public void CollapsePaths_RootInADifferentCase_StillMatches()
    {
        string collapsed = Redactor.CollapsePaths(@"c:\users\username\appdata\local\craftingcalculator\Exports\x.ccdata");

        collapsed.Should().Be("<path>.ccdata");
    }

    [Test]
    public void CollapsePaths_DatasetNameWithASpace_LeavesNoneOfItBehind()
    {
        // Stopping at whitespace would leave "Age-20260920.ccdata" in the log, which is half a dataset name.
        string collapsed = Redactor.CollapsePaths($@"'{WindowsRoot}\Exports\Bronze Age-20260920.ccdata'");

        collapsed.Should().NotContain("Bronze").And.NotContain("Age").And.Be("'<path>.ccdata'");
    }

    [Test]
    public void CollapsePaths_PathEndingASentence_KeepsTheFullStop()
    {
        string collapsed = Redactor.CollapsePaths($@"Nothing at {WindowsRoot}\Logs.");

        collapsed.Should().Be("Nothing at <path>.");
    }

    [Test]
    public void CollapsePaths_TextWithNoRootedPath_IsUnchanged()
    {
        const string message = "The blueprint graph exceeded the maximum depth of 64.";

        Redactor.CollapsePaths(message).Should().BeSameAs(message);
    }

    [Test]
    public void CollapsePaths_WithNoRoots_IsUnchanged()
    {
        LogRedactor redactor = new([]);

        redactor.CollapsePaths(@"C:\Users\UserName\file.txt").Should().Be(@"C:\Users\UserName\file.txt");
    }

    [Test]
    public void MaskQuoted_ReplacesASingleQuotedValue()
    {
        LogRedactor.MaskQuoted("The category 'Ores and Metals' already exists")
            .Should().Be("The category '***' already exists");
    }

    [Test]
    public void MaskQuoted_ReplacesADoubleQuotedValue()
    {
        LogRedactor.MaskQuoted("Unexpected token \"Bronze Axe\" in the file")
            .Should().Be("Unexpected token \"***\" in the file");
    }

    [Test]
    public void MaskQuoted_LeavesAnApostropheInsideAWordAlone()
    {
        const string message = "Your export couldn't be saved and wasn't written";

        LogRedactor.MaskQuoted(message).Should().Be(message);
    }

    [Test]
    public void MaskMessage_KeyNotFoundException_MasksTheKey()
    {
        Exception exception = new KeyNotFoundException("The given key 'Bronze Ingot' was not present in the dictionary.");

        LogRedactor.MaskMessage(exception).Should().Be("The given key '***' was not present in the dictionary.");
    }

    [Test]
    public void MaskMessage_SqliteException_IsKeptInFull()
    {
        // The quoted part is the schema, which is ours: losing it would leave no way to tell one constraint
        // failure from another.
        Exception exception = new SqliteException("SQLite Error 19: 'UNIQUE constraint failed: Components.Name'.", 19);

        LogRedactor.MaskMessage(exception).Should().Contain("UNIQUE constraint failed: Components.Name");
    }
}
