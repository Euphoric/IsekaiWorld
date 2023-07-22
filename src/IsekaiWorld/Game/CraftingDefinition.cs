using System;
using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld.Game;

public record CraftingDefinition(
    String Id,
    ItemDefinition Item,
    String Title
)
{
    public override string ToString()
    {
        return $"[{Id}]";
    }
}


public static class CraftingDefinitions
{
    public static readonly CraftingDefinition WoodenSpear = new("Core.Crafting.Weapons.WoodenSpear", ItemDefinitions.WoodenSpear, "Craft wooden spear");
    
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