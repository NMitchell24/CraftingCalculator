using CraftingCalculator.Application.Common.Utils;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Utils;

[TestFixture]
public class RouteUtilTests
{
    [TestCase(null, ExpectedResult = "")]
    [TestCase("", ExpectedResult = "")]
    [TestCase("  ", ExpectedResult = "")]
    [TestCase("/", ExpectedResult = "")]
    [TestCase("/dataset/Blueprint/3/", ExpectedResult = "dataset/blueprint/3")]
    [TestCase("dataset/Blueprint/0?copyFrom=7", ExpectedResult = "dataset/blueprint/0")]
    [TestCase("settings#appearance", ExpectedResult = "settings")]
    [TestCase("help?x=1#top", ExpectedResult = "help")]
    public string NormalizeRoute_ARoute_IsLowercasedWithoutSlashesQueryOrFragment(string? route) =>
        RouteUtil.NormalizeRoute(route);
}
