namespace CraftingCalculator.Model.Data
{
    public class RecipeFilterData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public RecipeFilterData()
        {
            Id = default!;
            Name = default!;
            Description = default!;
        }
    }
}
