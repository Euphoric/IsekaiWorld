using Godot;

public static class SurfaceDefinitions
{
    public static SurfaceDefinition Empty = new SurfaceDefinition(0, Colors.Black, false);
    public static SurfaceDefinition Grass = new SurfaceDefinition(1, Colors.DarkGreen, true);
    public static SurfaceDefinition Dirt = new SurfaceDefinition(2, Colors.SaddleBrown, true);
    
    public static SurfaceDefinition RockWall = new SurfaceDefinition(3, Color.Color8(69, 67, 63), false);
    public static SurfaceDefinition StoneWall = new SurfaceDefinition(4, Color.Color8(133, 133, 133), false);
    public static SurfaceDefinition WoodenWall = new SurfaceDefinition(5, Color.Color8(189, 116, 38), false);
}