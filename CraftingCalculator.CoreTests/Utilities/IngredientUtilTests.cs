using Microsoft.VisualStudio.TestTools.UnitTesting;
using CraftingCalculator.ViewModel.Ingredients;

namespace CraftingCalculator.Utilities.Tests
{
    [TestClass()]
    public class IngredientUtilTests
    {
        [TestMethod()]
        public void CombineIngredientsTest()
        {
            Ingredient i = new Ingredient()
            {
                Id = 1,
                Name = "Test",
                Description = "Test"
            };

            IngredientMap src = new IngredientMap();
            src.Add(i, 5);

            IngredientMap dest = new IngredientMap();
            dest.Add(i, 10);

            IngredientMap result = IngredientUtil.CombineIngredients(src, dest, 1);

            Assert.AreEqual(result.IngredientList.Count, 1);
            Assert.AreEqual(result.IngredientList[0].Ingredient.Name, i.Name);
            Assert.AreEqual(result.IngredientList[0].Quantity, 15);
        }
    }
}