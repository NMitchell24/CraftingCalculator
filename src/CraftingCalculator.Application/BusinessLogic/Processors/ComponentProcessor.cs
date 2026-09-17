using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

public static class ComponentProcessor
{
    public static ComponentMap CombineComponents(ComponentMap source, ComponentMap dest, long multiplier)
    {
        ComponentMap ret = dest.Clone();

        foreach (ComponentQuantity component in source.ComponentList)
        {
            ret.Add(component.Component, (component.Quantity * multiplier));
        }

        return ret;
    }
}
