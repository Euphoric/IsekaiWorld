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

    public void GenerateMap()
    {
        var surfaceNoise = new Simplex.Noise(){Seed = 123};
        var rockWallNoise = new Simplex.Noise(){Seed = 654};
        
        foreach (var cell in Cells)
        {
            var center = cell.Position.Center(1000);
            var isRockWall = rockWallNoise.CalcPixel2D(
                Mathf.CeilToInt(center.x),
                Mathf.CeilToInt(center.y), 1 / 1000f * 0.04f) < -0.5;
            
            var isGrass = surfaceNoise.CalcPixel2D(
                              Mathf.CeilToInt(center.x),
                              Mathf.CeilToInt(center.y), 1 / 1000f * 0.04f) <
                          0;
                          
            var surface = isRockWall ? 3 : isGrass ? 1 : 2;
            cell.Surface = surface;
        }
    }
}