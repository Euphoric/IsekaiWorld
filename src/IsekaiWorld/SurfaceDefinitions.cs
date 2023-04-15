using Godot;

namespace IsekaiWorld;

public static class SurfaceDefinitions
{
    public static readonly SurfaceDefinition Empty = new("Core.Empty", Colors.Black, false);
    public static readonly SurfaceDefinition Grass = new("Core.Natural.Grass", Colors.DarkGreen, true);
    public static readonly SurfaceDefinition Dirt = new("Core.Natural.Dirt", Colors.SaddleBrown, true);
    public static readonly SurfaceDefinition RoughStone = new("Core.Natural.RoughStone", new Color("9E9991"), true);
    
    public static readonly SurfaceDefinition TileFloor = new("Core.Floor.Tile", Color.Color8(133, 133, 133), true);
}