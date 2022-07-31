using System.Collections.Generic;
using System.Linq;

public static class ConstructionDefinitions
{
    public static readonly ConstructionDefinition StoneWall = new ConstructionDefinition("Core.Wall.Stone", "Stone wall", BuildingDefinitions.StoneWall.Id, null, 120);
    public static readonly ConstructionDefinition WoodenWall = new ConstructionDefinition("Core.Wall.Wood", "Wooden Wall", BuildingDefinitions.WoodenWall.Id, null, 60);
    
    public static readonly ConstructionDefinition StockpileZone = new ConstructionDefinition("Core.Zone.Stockpile", "Stockpile", BuildingDefinitions.StockpileZone.Id, null, 0);
    
    public static readonly ConstructionDefinition TileFloor = new ConstructionDefinition("Core.Floor.Tile", "Tile Floor", null, SurfaceDefinitions.TileFloor.Id, 60);
    
    public static readonly ConstructionDefinition WoodenChair = new ConstructionDefinition("Core.Furniture.Chair.Wood", "Wooden Chair", BuildingDefinitions.WoodenChair.Id, null, 120);
    public static readonly ConstructionDefinition WoodenBed = new ConstructionDefinition("Core.Furniture.Bed.Wood", "Wooden Bed", BuildingDefinitions.WoodenBed.Id, null, 120);
    public static readonly ConstructionDefinition TableStoveFueled = new ConstructionDefinition("Core.Production.StoveFueled", "Fueled stove", BuildingDefinitions.TableStoveFueled.Id, null, 120);
    
    private static readonly Dictionary<string, ConstructionDefinition> DefinitionsMap =
        new Dictionary<string, ConstructionDefinition>
        {
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall },
            
            {StockpileZone.Id, StockpileZone},
            
            { TileFloor.Id, TileFloor },
            { WoodenChair.Id, WoodenChair },
            { WoodenBed.Id, WoodenBed},
            { TableStoveFueled.Id, TableStoveFueled}
        };
    
    public static IReadOnlyList<ConstructionDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static ConstructionDefinition GetById(string id)
    {
        return DefinitionsMap[id];
    }
}