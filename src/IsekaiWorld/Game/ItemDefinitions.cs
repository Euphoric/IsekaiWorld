using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld.Game;

public static class ItemDefinitions
{
    public static ItemDefinition Wood { get; } = new("Core.Thing.Resource.Wood", "Wood", "res://Textures/Thing/WoodLog/WoodLog_c.png");
    public static ItemDefinition Grains { get; } = new("Core.Thing.Resource.Grains", "Grains", "res://Textures/Thing/Grains/Grains.png");

    public static ItemDefinition WoodenSpear { get; } = new ("Core.Thing.Equipment.Weapon.Spear.Wooden", "Wooden spear", "res://Textures/Thing/Equipment/Spear.png");

    private static readonly IReadOnlyDictionary<string, ItemDefinition> DefinitionsById = new Dictionary<string, ItemDefinition>()
    {
        { Wood.Id, Wood },
        { Grains.Id, Grains },
        { WoodenSpear.Id, WoodenSpear }
    };

    public static IReadOnlyList<ItemDefinition> Definitions { get; } = DefinitionsById.Values.ToList();

    public static ItemDefinition GetById(string itemId)
    {
        return DefinitionsById[itemId];
    }
}