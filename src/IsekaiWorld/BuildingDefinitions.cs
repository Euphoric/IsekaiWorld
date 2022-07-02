using System.Collections.Generic;
using System.Linq;
using Godot;

public static class BuildingDefinitions
{
    public static readonly BuildingDefinition RockWall = new BuildingDefinition("Core.Rock.Wall", "Rock Wall", Color.Color8(69, 67, 63), new Dictionary<HexagonDirection, string> { { HexagonDirection.Right, "res://Textures/Wall/wall texture.svg"}}, true);
    public static readonly BuildingDefinition StoneWall = new BuildingDefinition("Core.Wall.Stone", "Stone Wall", Color.Color8(133, 133, 133), new Dictionary<HexagonDirection, string> { { HexagonDirection.Right,"res://Textures/Wall/wall texture.svg" }},true);
    public static readonly BuildingDefinition WoodenWall = new BuildingDefinition("Core.Wall.Wood", "Wooden Wall", Color.Color8(189, 116, 38), new Dictionary<HexagonDirection, string> { { HexagonDirection.Right, "res://Textures/Wall/wall texture.svg" }}, true);

    public static readonly BuildingDefinition StockpileZone = new BuildingDefinition("Core.Zone.Stockpile", "Stockpile", Color.Color8(3, 252, 252, 128), new Dictionary<HexagonDirection, string> { { HexagonDirection.Right, "res://Textures/Zone/stockpile.svg" }}, true);
    
    public static readonly BuildingDefinition WoodenChair = new BuildingDefinition("Core.Furniture.Chair.Wood", "Wooden Chair", Color.Color8(189, 116, 38), new Dictionary<HexagonDirection, string>
    {
        { HexagonDirection.Right,"res://Textures/Furniture/DiningChair_east.png"},
        { HexagonDirection.BottomRight,"res://Textures/Furniture/DiningChair_south.png"},
        { HexagonDirection.BottomLeft,"res://Textures/Furniture/DiningChair_south.png"},
        { HexagonDirection.Left,"res://Textures/Furniture/DiningChair_east.png"},
        { HexagonDirection.TopLeft,"res://Textures/Furniture/DiningChair_north.png"},
        { HexagonDirection.TopRight,"res://Textures/Furniture/DiningChair_north.png"}
    });
    public static readonly BuildingDefinition WoodenBed = new BuildingDefinition("Core.Furniture.Bed.Wood", "Wooden Bed", Color.Color8(189, 116, 38), new Dictionary<HexagonDirection, string>
    {
        { HexagonDirection.Right,"res://Textures/Furniture/Bed_east.png"},
        { HexagonDirection.BottomRight,"res://Textures/Furniture/Bed_south.png"},
        { HexagonDirection.BottomLeft,"res://Textures/Furniture/Bed_south.png"},
        { HexagonDirection.Left,"res://Textures/Furniture/Bed_east.png"},
        { HexagonDirection.TopLeft,"res://Textures/Furniture/Bed_north.png"},
        { HexagonDirection.TopRight,"res://Textures/Furniture/Bed_north.png"}
    });
    public static readonly BuildingDefinition TableStoveFueled = new BuildingDefinition("Core.Production.StoveFueled", "Fueled stove", Colors.White, new Dictionary<HexagonDirection, string>
    {
        { HexagonDirection.Right,"res://Textures/Production/TableStoveFueled_east.png"},
        { HexagonDirection.BottomRight,"res://Textures/Production/TableStoveFueled_south.png"},
        { HexagonDirection.BottomLeft,"res://Textures/Production/TableStoveFueled_south.png"},
        { HexagonDirection.Left,"res://Textures/Production/TableStoveFueled_east.png"},
        { HexagonDirection.TopLeft,"res://Textures/Production/TableStoveFueled_north.png"},
        { HexagonDirection.TopRight,"res://Textures/Production/TableStoveFueled_north.png"}
    });
    
    private static readonly Dictionary<string, BuildingDefinition> DefinitionsMap =
        new Dictionary<string, BuildingDefinition>
        {
            { RockWall.Id, RockWall },
            { WoodenWall.Id, WoodenWall },
            
            {StockpileZone.Id, StockpileZone},
            
            { StoneWall.Id, StoneWall },
            { WoodenChair.Id, WoodenChair },
            { WoodenBed.Id, WoodenBed },
            {TableStoveFueled.Id, TableStoveFueled}
        };

    public static IReadOnlyList<BuildingDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static BuildingDefinition GetById(string buildingDefinitionId)
    {
        return DefinitionsMap[buildingDefinitionId];
    }
}