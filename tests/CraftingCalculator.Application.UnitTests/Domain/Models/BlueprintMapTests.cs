using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Domain.Models;

[TestFixture]
public class BlueprintMapTests
{
    [Test]
    public void Remove_AnEntrySharingItsNameWithAnother_LeavesTheOtherInPlace()
    {
        BlueprintMap map = new();
        map.Add(new BlueprintModel { Id = 1, Name = "Bronze" }, 1);
        map.Add(new BlueprintModel { Id = 2, Name = "Iron" }, 3);

        // A rename after both were added, the way a reload swaps a saved blueprint into its entry.
        BlueprintQuantity iron = map.BlueprintList[1];
        iron.Blueprint = new BlueprintModel { Id = 2, Name = "Bronze" };

        map.Remove(map.BlueprintList[0]);

        map.BlueprintList.Should().ContainSingle().Which.Should().BeSameAs(iron);
    }
}
