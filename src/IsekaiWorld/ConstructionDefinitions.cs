using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public static class ConstructionDefinitions
{
    public static readonly ConstructionDefinition TestWoodenWall = new("Test.Wall.Stone", "Wooden wall", BuildingDefinitions.WoodenWall, null, 60, ItemDefinitions.Wood); 
    
    public static readonly ConstructionDefinition StoneWall = new("Core.Wall.Stone", "Stone wall", BuildingDefinitions.StoneWall, null, 120);
    public static readonly ConstructionDefinition WoodenWall = new("Core.Wall.Wood", "Wooden Wall", BuildingDefinitions.WoodenWall, null, 60, ItemDefinitions.Wood);
    
    public static readonly ConstructionDefinition StockpileZone = new("Core.Zone.Stockpile", "Stockpile", BuildingDefinitions.StockpileZone, null, 0);
    
    public static readonly ConstructionDefinition TileFloor = new("Core.Floor.Tile", "Tile Floor", null, SurfaceDefinitions.TileFloor, 60);
    
    public static readonly ConstructionDefinition WoodenChair = new("Core.Furniture.Chair.Wood", "Wooden Chair", BuildingDefinitions.WoodenChair, null, 120, ItemDefinitions.Wood);
    public static readonly ConstructionDefinition WoodenBed = new("Core.Furniture.Bed.Wood", "Wooden Bed", BuildingDefinitions.WoodenBed, null, 120, ItemDefinitions.Wood);
    public static readonly ConstructionDefinition TableStoveFueled = new("Core.Production.StoveFueled", "Fueled stove", BuildingDefinitions.TableStoveFueled, null, 120);
    
    public static readonly ConstructionDefinition CraftingDesk = new("Core.Production.CraftingDesk", "Crafting desk", BuildingDefinitions.CraftingDesk, null, 0);
    
    private static readonly Dictionary<string, ConstructionDefinition> DefinitionsMap =
        new()
        {
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall },
            
            {StockpileZone.Id, StockpileZone},
            
            { TileFloor.Id, TileFloor },
            { WoodenChair.Id, WoodenChair },
            { WoodenBed.Id, WoodenBed},
            { TableStoveFueled.Id, TableStoveFueled},
            { CraftingDesk.Id, CraftingDesk}
        };
    
    public static IReadOnlyList<ConstructionDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static ConstructionDefinition GetById(string id)
    {
        return DefinitionsMap[id];
    }
}