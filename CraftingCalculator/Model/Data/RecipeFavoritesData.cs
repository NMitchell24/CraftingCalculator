namespace CraftingCalculator.Model.Data
{
    public class RecipeFavoritesData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public RecipeFavoritesData()
        {
            Id = default!;
            Name = default!;
        }
    }
}
