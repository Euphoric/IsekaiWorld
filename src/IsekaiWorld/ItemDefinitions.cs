using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public static class ItemDefinitions
{
    public static ItemDefinition Wood { get; } = new ItemDefinition("Core.Thing.Resource.Wood", "Wood", "res://Textures/Thing/WoodLog/WoodLog_c.png");
    public static ItemDefinition Grains { get; } = new ItemDefinition("Core.Thing.Resource.Grains", "Grains", "res://Textures/Thing/Grains/Grains.png");

    private static readonly IReadOnlyDictionary<string, ItemDefinition> DefinitionsById = new Dictionary<string, ItemDefinition>()
    {
        { Wood.Id, Wood },
        { Grains.Id, Grains },
    };

    public static IReadOnlyList<ItemDefinition> Definitions { get; } = DefinitionsById.Values.ToList();

    public static ItemDefinition GetById(string itemId)
    {
        return DefinitionsById[itemId];
    }
}