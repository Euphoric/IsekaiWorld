public class MapCell
{
    public HexCubeCoord Position { get; }

    public MapCell(HexCubeCoord position)
    {
        Position = position;
    }

    public int Surface { get; set; }
}