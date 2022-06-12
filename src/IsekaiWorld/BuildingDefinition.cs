
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BuildingDefinition
{
    public string Id { get; }
    public string Label { get; }
    public Color Color { get; }

    public BuildingDefinition(string id, string label, Color color)
    {
        Id = id;
        Label = label;
        Color = color;
    }
}

public static class BuildingDefinitions
{
    public static readonly BuildingDefinition RockWall = new BuildingDefinition("Core.Rock.Wall", "Rock Wall", Color.Color8(69, 67, 63));
    public static readonly BuildingDefinition StoneWall = new BuildingDefinition("Core.Wall.Stone", "Stone Wall", Color.Color8(133, 133, 133));
    public static readonly BuildingDefinition WoodenWall = new BuildingDefinition("Core.Wall.Wood", "Wooden Wall", Color.Color8(189, 116, 38));

    private static readonly Dictionary<string, BuildingDefinition> DefinitionsMap =
        new Dictionary<string, BuildingDefinition>
        {
            { RockWall.Id, RockWall },
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall }
        };

    public static IReadOnlyList<BuildingDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static BuildingDefinition GetById(string buildingDefinitionId)
    {
        return DefinitionsMap[buildingDefinitionId];
    }
}

public class ConstructionDefinition
{
    public string Id { get; }
    public string Label { get; }
    public string PlaceBuildingId { get; }
    public string PlaceFloorId { get; }

    public ConstructionDefinition(string id, string label, string placeBuildingId, string placeFloorId)
    {
        Id = id;
        Label = label;
        PlaceBuildingId = placeBuildingId;
        PlaceFloorId = placeFloorId;
    }
}

public static class ConstructionDefinitions
{
    public static readonly ConstructionDefinition StoneWall = new ConstructionDefinition("Core.Wall.Stone", "Stone wall", BuildingDefinitions.StoneWall.Id, null);
    public static readonly ConstructionDefinition WoodenWall = new ConstructionDefinition("Core.Wall.Wood", "Wooden Wall", BuildingDefinitions.WoodenWall.Id, null);
    
    public static readonly ConstructionDefinition TileFloor = new ConstructionDefinition("Core.Floor.Tile", "Tile Floor", null, SurfaceDefinitions.TileFloor.Id);
    
    private static readonly Dictionary<string, ConstructionDefinition> DefinitionsMap =
        new Dictionary<string, ConstructionDefinition>
        {
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall },
            { TileFloor.Id, TileFloor }
        };
    
    public static IReadOnlyList<ConstructionDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static ConstructionDefinition GetById(string id)
    {
        return DefinitionsMap[id];
    }
}