using System.Collections.Generic;
using System.Linq;
using Godot;

public class HexMap
{
    public HexMap(int size)
    {
        Hexes = MapCoordinates(size).ToList();
        Cells = Hexes.Select(pos => new MapCell(pos)).ToList();
    }

    public IReadOnlyList<HexCubeCoord> Hexes { get; }
    public IReadOnlyList<MapCell> Cells { get; }

    private bool _mapChangeDirty = true; 
    
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

    public void SetCell(HexCubeCoord position, SurfaceDefinition surface)
    {
        var cell = CellForPosition(position);
        cell.Surface = surface;

        _mapChangeDirty = true;
    }

    public MapCell CellForPosition(HexCubeCoord position)
    {
        return Cells.First(c => c.Position == position);
    }

    public void Update(HexagonalMap map)
    {
        if (_mapChangeDirty)
        {
            map.RefreshGameMap();
            _mapChangeDirty = false;
        }
    }
}