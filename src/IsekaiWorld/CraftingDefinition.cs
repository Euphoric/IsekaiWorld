using System;
using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public record CraftingDefinition(
    String Id,
    ItemDefinition Item
);


public static class CraftingDefinitions
{
    public static readonly CraftingDefinition WoodenSpear = new("Core.Crafting.Weapons.WoodenSpear", ItemDefinitions.WoodenSpear);
    
    private static Dictionary<string, CraftingDefinition> DefinitionsMap { get; } =
        new()
        {
            { WoodenSpear.Id, WoodenSpear },
        };

    public static IReadOnlyList<CraftingDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static CraftingDefinition GetById(string buildingDefinitionId)
    {
        return DefinitionsMap[buildingDefinitionId];
    }
}