
using System.Collections.Generic;
using System.Linq;

public class BuildingDefinition
{
    public string Id { get; }
    public string Label { get; }
    public SurfaceDefinition Surface { get; }

    public BuildingDefinition(string id, string label, SurfaceDefinition surface)
    {
        Id = id;
        Label = label;
        Surface = surface;
    }
}

public static class BuildingDefinitions
{
    public static readonly BuildingDefinition StoneWall = new BuildingDefinition("Core.Wall.Stone", "Stone Wall", SurfaceDefinitions.StoneWall);
    public static readonly BuildingDefinition WoodenWall = new BuildingDefinition("Core.Wall.Wood", "Wooden Wall", SurfaceDefinitions.WoodenWall);
    public static readonly BuildingDefinition TileFloor = new BuildingDefinition("Core.Floor.Tile", "Tile Floor", SurfaceDefinitions.TileFloor);

    private static readonly Dictionary<string, BuildingDefinition> DefinitionsMap =
        new Dictionary<string, BuildingDefinition>
        {
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall },
            { TileFloor.Id, TileFloor }
        };

    public static IReadOnlyList<BuildingDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static BuildingDefinition GetById(string buildingDefinitionId)
    {
        return DefinitionsMap[buildingDefinitionId];
    }
}