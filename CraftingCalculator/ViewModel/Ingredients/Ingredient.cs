﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.ViewModel.Ingredients
{
    public class Ingredient : IBaseDataRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public string Tooltip
        {
            get
            {
                return Description;
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
            ret.Name = ret.Name + " - Copy";
            ret.Id = 0;

            return ret;
        }
    }
}