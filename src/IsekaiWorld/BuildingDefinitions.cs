using System.Collections.Generic;
using System.Linq;
using Godot;

public static class BuildingDefinitions
{
    public static readonly BuildingDefinition RockWall = new BuildingDefinition("Core.Rock.Wall", "Rock Wall", Color.Color8(69, 67, 63),"res://Textures/Wall/wall texture.svg", true);
    public static readonly BuildingDefinition StoneWall = new BuildingDefinition("Core.Wall.Stone", "Stone Wall", Color.Color8(133, 133, 133), "res://Textures/Wall/wall texture.svg",true);
    public static readonly BuildingDefinition WoodenWall = new BuildingDefinition("Core.Wall.Wood", "Wooden Wall", Color.Color8(189, 116, 38),"res://Textures/Wall/wall texture.svg", true);

    public static readonly BuildingDefinition WoodenChair = new BuildingDefinition("Core.Furniture.Chair.Wood", "Wooden Chair", Color.Color8(189, 116, 38), "res://Textures/Furniture/DiningChair_south.png");

    private static readonly Dictionary<string, BuildingDefinition> DefinitionsMap =
        new Dictionary<string, BuildingDefinition>
        {
            { RockWall.Id, RockWall },
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall },
            { WoodenChair.Id, WoodenChair}
        };

    public static IReadOnlyList<BuildingDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static BuildingDefinition GetById(string buildingDefinitionId)
    {
        return DefinitionsMap[buildingDefinitionId];
    }
}