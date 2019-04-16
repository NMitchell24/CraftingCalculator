﻿using System.Collections.Generic;
using LiteDB;

namespace CraftingCalculator.Model.LiteDB
{
    public class RecipeData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [BsonRef("RecipeFilters")]
        public RecipeFilterData Filter { get; set; }

        [BsonRef("IngredientQuantities")]
        public List<IngredientQuantityData> Ingredients { get; set; }
    }
}
