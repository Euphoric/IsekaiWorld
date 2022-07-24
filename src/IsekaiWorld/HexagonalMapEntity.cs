using System.Collections.Generic;
using System.Linq;

public class HexagonalMapEntity
{
    public int Size { get; }

    public HexagonalMapEntity(int size)
    {
        Size = size;
        Hexes = MapCoordinates(size).ToList();
        Cells = Hexes.Select(pos => new MapCell(pos)).ToList();
        _cellsByPosition = Cells.ToDictionary(c => c.Position, c => c);
    }

    private readonly IReadOnlyDictionary<HexCubeCoord, MapCell> _cellsByPosition;
    public IReadOnlyList<HexCubeCoord> Hexes { get; }
    public IReadOnlyList<MapCell> Cells { get; }

    private static IEnumerable<HexCubeCoord> MapCoordinates(int size)
    {
        for (int r = -size; r <= size; r++)
        {
            for (int q = -size; q <= size; q++)
            {
                var s = 0 - r - q;
                if (!(s >= -size && s <= size))
                    continue;

                yield return new HexCubeCoord(r, q, s);
            }
        }
    }

    public void SetCellSurface(HexCubeCoord position, SurfaceDefinition surface)
    {
        _cellsByPosition[position].Surface = surface;
    }
    
    public bool IsWithinMap(HexCubeCoord hexCubeCoord)
    {
        return hexCubeCoord.DistanceFrom(HexCubeCoord.Zero) <= Size;
    }
    
    public MapCell CellForPosition(HexCubeCoord position)
    {
        return _cellsByPosition[position];
    }
}