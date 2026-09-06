namespace CraftingCalculator.Domain.Entities
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
