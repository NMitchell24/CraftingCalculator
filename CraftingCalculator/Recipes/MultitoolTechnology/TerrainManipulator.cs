﻿using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class TerrainManipulator : ComplexRecipe
    {
        public TerrainManipulator()
        {
            Name = "Terrain Manipulator";
            Type = "Multitool Module";
            ChildRecipes.Add(new DihydrogenJelly(), 1);
            ChildRecipes.Add(new CarbonNanotubes(), 2);
        }
    }
}
