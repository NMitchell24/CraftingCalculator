
namespace CraftingCalculator.ViewModel.Recipes
{
    /// <summary>
    /// Simple Class to define the UI Model for RecipeFilters
    /// </summary>
    public class RecipeFilter : IBaseDataRecord
    {
        public const string ALL = "All";
        public string Name { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string Tooltip { get; set; }
        public DataType Type
        {
            get
            {
                return DataType.RecipeType;
            }
            //Don't allow this to be changed as it should remain static
            set { }
        }

        public IBaseDataRecord Clone()
        {
            RecipeFilter clone = new RecipeFilter()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description
            };

            return clone;
        }

        public IBaseDataRecord CopyForSave()
        {
            RecipeFilter ret = (RecipeFilter)Clone();
            ret.Name = ret.Name + " - Copy";
            ret.Id = 0;

            return ret;
        }
    }
}
