namespace CraftingCalculator.Model.Data
{
    public class IngredientData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }

        public IngredientData()
        {
            Id = default!;
            Name = default!;
            Description = default!;
            Cost = default!;
        }
    }
}
