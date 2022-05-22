using Godot;

public static class SurfaceDefinitions
{
    public static SurfaceDefinition Empty = new SurfaceDefinition(0, Colors.Black, false);
    public static SurfaceDefinition Grass = new SurfaceDefinition(1, Colors.DarkGreen, true);
    public static SurfaceDefinition Dirt = new SurfaceDefinition(2, Colors.SaddleBrown, true);
    public static SurfaceDefinition RockWall = new SurfaceDefinition(3, Color.Color8(31, 28, 23), false);
}