using Godot;

public static class SurfaceDefinitions
{
    public static SurfaceDefinition Empty = new SurfaceDefinition("Core.Empty", Colors.Black, false);
    public static SurfaceDefinition Grass = new SurfaceDefinition("Core.Natural.Grass", Colors.DarkGreen, true);
    public static SurfaceDefinition Dirt = new SurfaceDefinition("Core.Natural.Dirt", Colors.SaddleBrown, true);

    public static SurfaceDefinition TileFloor = new SurfaceDefinition("Core.Floor.Tile", Color.Color8(133, 133, 133), true);
}