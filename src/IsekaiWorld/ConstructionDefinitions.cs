using System.Collections.Generic;
using System.Linq;

public static class ConstructionDefinitions
{
    public static readonly ConstructionDefinition StoneWall = new ConstructionDefinition("Core.Wall.Stone", "Stone wall", BuildingDefinitions.StoneWall.Id, null);
    public static readonly ConstructionDefinition WoodenWall = new ConstructionDefinition("Core.Wall.Wood", "Wooden Wall", BuildingDefinitions.WoodenWall.Id, null);
    
    public static readonly ConstructionDefinition TileFloor = new ConstructionDefinition("Core.Floor.Tile", "Tile Floor", null, SurfaceDefinitions.TileFloor.Id);
    
    public static readonly ConstructionDefinition WoodenChair = new ConstructionDefinition("Core.Furniture.Chair.Wood", "Wooden Chair", BuildingDefinitions.WoodenChair.Id, null);

    private static readonly Dictionary<string, ConstructionDefinition> DefinitionsMap =
        new Dictionary<string, ConstructionDefinition>
        {
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall },
            { TileFloor.Id, TileFloor },
            { WoodenChair.Id, WoodenChair }
        };
    
    public static IReadOnlyList<ConstructionDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static ConstructionDefinition GetById(string id)
    {
        return DefinitionsMap[id];
    }
}