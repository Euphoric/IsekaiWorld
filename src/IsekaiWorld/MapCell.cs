public class MapCell
{
    public HexCubeCoord Position { get; }

    public MapCell(HexCubeCoord position)
    {
        Position = position;
    }

    public SurfaceDefinition Surface { get; set; } = SurfaceDefinitions.Empty;
}