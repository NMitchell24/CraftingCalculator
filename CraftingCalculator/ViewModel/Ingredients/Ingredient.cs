using System;

namespace CraftingCalculator.ViewModel.Ingredients
{
    public class Ingredient : IBaseDataRecord
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Cost { get; set; }
        public string Tooltip
        {
            get
            {
                string ret = Description ?? "";
                if (Cost > 0)
                {
                    ret = ret +
                        Environment.NewLine + Environment.NewLine +
                        "Cost Per Item: " + string.Format("{0:C2}", Cost);
                }
                return ret;
            }
            set { }
        }       
        public DataType Type
        {
            get
            {
                return DataType.Ingredient;
            }
            //Don't allow this to be changed as it should remain static.
            set { }
        }

        public IBaseDataRecord Clone()
        {
            Ingredient clone = new Ingredient()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Cost = this.Cost
            };

            return clone;
        }

        public IBaseDataRecord CopyForSave()
        {
            Ingredient ret = (Ingredient)Clone();
            ret.Name += " - Copy";
            ret.Id = 0;

            return ret;
        }
    }
}
