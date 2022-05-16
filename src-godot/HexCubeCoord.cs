using System;
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
        var noise = new Simplex.Noise();
        foreach (var cell in Cells)
        {
            var center = cell.Position.Center(1000);
            var isGrass = noise.CalcPixel2D(
                              Mathf.CeilToInt(center.x),
                              Mathf.CeilToInt(center.y), 1 / 1000f * 0.04f) <
                          128;
            var surface = isGrass ? 1 : 2;
            cell.Surface = surface;
        }
    }
}

public class MapCell
{
    public HexCubeCoord Position { get; }

    public MapCell(HexCubeCoord position)
    {
        Position = position;
    }

    public int Surface { get; set; }
}

public readonly struct HexCubeCoord
{
    public int R { get; }
    public int Q { get; }
    public int S { get; }

    public HexCubeCoord(int r, int q, int s)
    {
        if (r + q + s != 0)
        {
            throw new Exception("Invalid hexagonal cube coordinates.");
        }

        R = r;
        Q = q;
        S = s;
    }

    public Vector2 Center(float size)
    {
        var x = size * (Mathf.Sqrt(3) * Q + Mathf.Sqrt(3) / 2 * R);
        var y = size * (3f / 2 * R);
        return new Vector2(x, y);
    }
}