using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld.Game;

public static class ConstructionDefinitions
{
    public static readonly ConstructionDefinition TestWoodenWall = new("Test.Wall.Stone", "Wooden wall", BuildingDefinitions.WoodenWall, null, 5, ItemDefinitions.Wood); 
    
    public static readonly ConstructionDefinition StoneWall = new("Core.Wall.Stone", "Stone wall", BuildingDefinitions.StoneWall, null, 10);
    public static readonly ConstructionDefinition WoodenWall = new("Core.Wall.Wood", "Wooden Wall", BuildingDefinitions.WoodenWall, null, 5, ItemDefinitions.Wood);
    
    public static readonly ConstructionDefinition StockpileZone = new("Core.Zone.Stockpile", "Stockpile", BuildingDefinitions.StockpileZone, null, 0);
    
    public static readonly ConstructionDefinition StoneTileFloor = new("Core.Floor.StoneTile", "Stone tile Floor", null, SurfaceDefinitions.StoneTileFloor, 3);

    public static readonly ConstructionDefinition WoodPlankFloor = new("Core.Floor.WoodPLank", "Wooden plank Floor", null, SurfaceDefinitions.WoodPlankFloor, 2, ItemDefinitions.Wood);
    
    public static readonly ConstructionDefinition WoodenChair = new("Core.Furniture.Chair.Wood", "Wooden Chair", BuildingDefinitions.WoodenChair, null, 5, ItemDefinitions.Wood);
    public static readonly ConstructionDefinition WoodenBed = new("Core.Furniture.Bed.Wood", "Wooden Bed", BuildingDefinitions.WoodenBed, null, 5, ItemDefinitions.Wood);
    public static readonly ConstructionDefinition TableStoveFueled = new("Core.Production.StoveFueled", "Fueled stove", BuildingDefinitions.TableStoveFueled, null, 5);
    
    public static readonly ConstructionDefinition CraftingDesk = new("Core.Production.CraftingDesk", "Crafting desk", BuildingDefinitions.CraftingDesk, null, 0);
    
    private static readonly Dictionary<string, ConstructionDefinition> DefinitionsMap =
        new()
        {
            { WoodenWall.Id, WoodenWall },
            { StoneWall.Id, StoneWall },
            
            {StockpileZone.Id, StockpileZone},
            
            { StoneTileFloor.Id, StoneTileFloor },
            { WoodPlankFloor.Id, WoodPlankFloor},
            
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