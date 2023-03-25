using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

public static class BuildingDefinitions
{
    public static BuildingDefinition RockWall { get; } = new("Core.Rock.Wall", "Rock Wall", Color.Color8(69, 67, 63),
        new Dictionary<HexagonDirection, string> { { HexagonDirection.Right, "res://Textures/Wall/wall texture.svg" } },
        true, true);

    public static BuildingDefinition StoneWall { get; } = new("Core.Wall.Stone", "Stone Wall",
        Color.Color8(133, 133, 133),
        new Dictionary<HexagonDirection, string> { { HexagonDirection.Right, "res://Textures/Wall/wall texture.svg" } },
        true, true, new[] { DesignationDefinitions.Deconstruct }.ToImmutableHashSet());

    public static BuildingDefinition WoodenWall { get; } = new("Core.Wall.Wood", "Wooden Wall",
        Color.Color8(189, 116, 38),
        new Dictionary<HexagonDirection, string> { { HexagonDirection.Right, "res://Textures/Wall/wall texture.svg" } },
        true, true, new[] { DesignationDefinitions.Deconstruct }.ToImmutableHashSet());

    public static BuildingDefinition StockpileZone { get; } = new("Core.Zone.Stockpile", "Stockpile",
        Color.Color8(3, 252, 252, 128),
        new Dictionary<HexagonDirection, string> { { HexagonDirection.Right, "res://Textures/Zone/stockpile.svg" } },
        true, allowedDesignations: new[] { DesignationDefinitions.Deconstruct }.ToImmutableHashSet());

    public static BuildingDefinition WoodenChair { get; } = new("Core.Furniture.Chair.Wood", "Wooden Chair",
        Color.Color8(189, 116, 38), new Dictionary<HexagonDirection, string>
        {
            { HexagonDirection.Right, "res://Textures/Furniture/DiningChair_east.png" },
            { HexagonDirection.BottomRight, "res://Textures/Furniture/DiningChair_bottomright.png" },
            { HexagonDirection.BottomLeft, "res://Textures/Furniture/DiningChair_bottomright.png" },
            { HexagonDirection.Left, "res://Textures/Furniture/DiningChair_east.png" },
            { HexagonDirection.TopLeft, "res://Textures/Furniture/DiningChair_topright.png" },
            { HexagonDirection.TopRight, "res://Textures/Furniture/DiningChair_topright.png" }
        }, allowedDesignations: new[] { DesignationDefinitions.Deconstruct }.ToImmutableHashSet());

    public static BuildingDefinition WoodenBed { get; } = new("Core.Furniture.Bed.Wood", "Wooden Bed",
        Color.Color8(189, 116, 38), new Dictionary<HexagonDirection, string>
        {
            { HexagonDirection.Right, "res://Textures/Furniture/Bed_east.png" },
            { HexagonDirection.BottomRight, "res://Textures/Furniture/Bed_bottomright.png" },
            { HexagonDirection.BottomLeft, "res://Textures/Furniture/Bed_bottomright.png" },
            { HexagonDirection.Left, "res://Textures/Furniture/Bed_east.png" },
            { HexagonDirection.TopLeft, "res://Textures/Furniture/Bed_topright.png" },
            { HexagonDirection.TopRight, "res://Textures/Furniture/Bed_topright.png" }
        }, allowedDesignations: new[] { DesignationDefinitions.Deconstruct }.ToImmutableHashSet());

    public static BuildingDefinition TableStoveFueled { get; } = new("Core.Production.StoveFueled", "Fueled stove",
        Colors.White, new Dictionary<HexagonDirection, string>
        {
            { HexagonDirection.Right, "res://Textures/Production/TableStoveFueled_east.png" },
            { HexagonDirection.BottomRight, "res://Textures/Production/TableStoveFueled_south.png" },
            { HexagonDirection.BottomLeft, "res://Textures/Production/TableStoveFueled_south.png" },
            { HexagonDirection.Left, "res://Textures/Production/TableStoveFueled_east.png" },
            { HexagonDirection.TopLeft, "res://Textures/Production/TableStoveFueled_north.png" },
            { HexagonDirection.TopRight, "res://Textures/Production/TableStoveFueled_north.png" }
        }, allowedDesignations: new[] { DesignationDefinitions.Deconstruct }.ToImmutableHashSet());

    public static BuildingDefinition CraftingDesk { get; } = new("Core.Production.CraftingDesk", "Crafting desk",
        Colors.SaddleBrown, new Dictionary<HexagonDirection, string>
        {
            { HexagonDirection.Right, "res://Textures/Placeholder/Wide_right.svg" },
            { HexagonDirection.BottomRight, "res://Textures/Placeholder/Wide_bottomright.svg" },
            { HexagonDirection.BottomLeft, "res://Textures/Placeholder/Wide_bottomleft.svg" },
            { HexagonDirection.Left, "res://Textures/Placeholder/Wide_left.svg" },
            { HexagonDirection.TopLeft, "res://Textures/Placeholder/Wide_topleft.svg" },
            { HexagonDirection.TopRight, "res://Textures/Placeholder/Wide_topright.svg" }
        }, allowedDesignations: new[] { DesignationDefinitions.Deconstruct }.ToImmutableHashSet());

    public static class Plant
    {
        public static BuildingDefinition TreeOak { get; } = new("Core.Tree.Oak", "Oak tree", Colors.White,
            new Dictionary<HexagonDirection, string>
            {
                { HexagonDirection.Left, "res://Textures/Plant/TreeOak.png" }
            },
            allowedDesignations: new[] { DesignationDefinitions.CutWood }.ToImmutableHashSet()
        );

        public static BuildingDefinition Haygrass { get; } = new("Core.Plant.Haygrass", "Haygrass", Colors.White,
            new Dictionary<HexagonDirection, string>
            {
                { HexagonDirection.Left, "res://Textures/Plant/Haygrass.png" }
            }
        );
        
        public static BuildingDefinition WildRice { get; } = new("Core.Plant.Rice", "Rice", Colors.White,
            new Dictionary<HexagonDirection, string>
            {
                { HexagonDirection.Left, "res://Textures/Plant/RicePlant_Grown.png" }
            }
        );
    }

    private static Dictionary<string, BuildingDefinition> DefinitionsMap { get; } =
        new()
        {
            { RockWall.Id, RockWall },
            { WoodenWall.Id, WoodenWall },

            { StockpileZone.Id, StockpileZone },

            { StoneWall.Id, StoneWall },
            { WoodenChair.Id, WoodenChair },
            { WoodenBed.Id, WoodenBed },
            { TableStoveFueled.Id, TableStoveFueled },
            { CraftingDesk.Id, CraftingDesk }
        };

    public static IReadOnlyList<BuildingDefinition> Definitions => DefinitionsMap.Values.ToList();

    public static BuildingDefinition GetById(string buildingDefinitionId)
    {
        return DefinitionsMap[buildingDefinitionId];
    }
}