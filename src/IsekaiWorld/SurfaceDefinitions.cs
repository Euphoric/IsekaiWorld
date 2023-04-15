using Godot;

namespace IsekaiWorld;

public static class SurfaceDefinitions
{
    public static readonly SurfaceDefinition Empty = new("Core.Empty", Colors.Black,null, 1, false);
    public static readonly SurfaceDefinition Grass = new("Core.Natural.Grass", Colors.White,"res://Textures/Surface/grass.png", 0.2f, true);
    public static readonly SurfaceDefinition Dirt = new("Core.Natural.Dirt", Colors.White,"res://Textures/Surface/dirt.jpg", 1.0f, true);
    public static readonly SurfaceDefinition RoughStone = new("Core.Natural.RoughStone", new Color("9E9991"),"res://Textures/Surface/RoughStone.png", 0.4f, true);
    
    public static readonly SurfaceDefinition TileFloor = new("Core.Floor.Tile", Color.Color8(133, 133, 133),"res://Textures/Surface/TilePatternEven_Floor.png", 0.05f, true);
}