﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace IsekaiWorld.Game;

public readonly struct HexCubeCoord : IEquatable<HexCubeCoord>
{
    public int R { get; }
    public int Q { get; }
    public int S { get; }

    public static HexCubeCoord Zero { get; } = new HexCubeCoord(0, 0, 0);

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

    public override string ToString()
    {
        return $"[R:{R},Q:{Q},S:{S}]";
    }

    private static HexCubeCoord CubeRound(float fracR, float fracQ, float fracS)
    {
        var q = Mathf.RoundToInt(fracQ);
        var r = Mathf.RoundToInt(fracR);
        var s = Mathf.RoundToInt(fracS);

        var qDiff = Mathf.Abs(q - fracQ);
        var rDiff = Mathf.Abs(r - fracR);
        var sDiff = Mathf.Abs(s - fracS);

        if (qDiff > rDiff && qDiff > sDiff)
            q = -r - s;
        else if (rDiff > sDiff)
            r = -q - s;
        else
            s = -q - r;

        return new HexCubeCoord(r, q, s);
    }

    public static HexCubeCoord FromPosition(Vector2 position, float size)
    {
        var q = (Mathf.Sqrt(3) / 3 * position.X - 1f / 3 * position.Y) / size;
        var r = (2f / 3 * position.Y) / size;
        var s = 0 - r - q;

        return CubeRound(r, q, s);
    }

    public static Vector2 HexCorner(HexCubeCoord coordinates, float size, int i)
    {
        var center = coordinates.Center(size);

        var angleDeg = 60 * i - 30;
        var angleRad = (float)(Math.PI / 180 * angleDeg);
        return new Vector2(center.X + size * Mathf.Cos(angleRad),
            center.Y + size * Mathf.Sin(angleRad));
    }

    public static HexCubeCoord operator +(HexCubeCoord position, HexagonDirection direction)
    {
        int r = position.R;
        int q = position.Q;
        int s = position.S;

        switch (direction)
        {
            case HexagonDirection.Left:
                q -= 1;
                s += 1;
                break;
            case HexagonDirection.Right:
                q += 1;
                s -= 1;
                break;
            case HexagonDirection.TopLeft:
                r -= 1;
                s += 1;
                break;
            case HexagonDirection.TopRight:
                q += 1;
                r -= 1;
                break;
            case HexagonDirection.BottomLeft:
                q -= 1;
                r += 1;
                break;
            case HexagonDirection.BottomRight:
                r += 1;
                s -= 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
        
        return new HexCubeCoord(r, q, s);
    }

    public IReadOnlyList<HexCubeCoord> Neighbors()
    {
        return new[]
        {
            this + HexagonDirection.Left,
            this + HexagonDirection.TopLeft,
            this + HexagonDirection.TopRight,
            this + HexagonDirection.Right,
            this + HexagonDirection.BottomRight,
            this + HexagonDirection.BottomLeft,
        };
    }
    
    public bool IsNextTo(HexCubeCoord position)
    {
        return Neighbors().Contains(position);
    }
    
    public int DistanceFrom(HexCubeCoord coord)
    {
        return (Math.Abs(this.Q - coord.Q) + Math.Abs(this.R - coord.R) + Math.Abs(this.S - coord.S)) / 2;
    }
    
    public bool Equals(HexCubeCoord other)
    {
        return R == other.R && Q == other.Q && S == other.S;
    }

    public override bool Equals(object? obj)
    {
        return obj is HexCubeCoord other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = R;
            hashCode = (hashCode * 397) ^ Q;
            hashCode = (hashCode * 397) ^ S;
            return hashCode;
        }
    }

    public static bool operator ==(HexCubeCoord left, HexCubeCoord right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HexCubeCoord left, HexCubeCoord right)
    {
        return !left.Equals(right);
    }

    public HexagonDirection DirectionTo(HexCubeCoord neighbor)
    {
        var diffR = neighbor.R - R;
        var diffS = neighbor.S - S;
        var diffQ = neighbor.Q - Q;

        if (diffR == 1 && diffS == -1)
        {
            return HexagonDirection.BottomRight;
        }
        if (diffR == 1 && diffQ == -1)
        {
            return HexagonDirection.BottomLeft;
        }
        if (diffR == -1 && diffS == 1)
        {
            return HexagonDirection.TopLeft;
        }
        if (diffR == -1 && diffQ == 1)
        {
            return HexagonDirection.TopRight;
        }

        if (diffQ == -1 && diffS == 1)
        {
            return HexagonDirection.Left;
        }

        if (diffQ == 1 && diffS == -1)
        {
            return HexagonDirection.Right;
        }

        throw new ArgumentException("Neighbor must be cell next to this one.");
    }

    public HexOffsetCoord ToOffset()
    {
        var col = Q + (R - (R & 1)) / 2;
        var row = R;
        return new HexOffsetCoord(col, row);
    }
    
    public static void FillRectangleBetweenHexes(HexCubeCoord fromHex, HexCubeCoord toHex,
        ICollection<HexCubeCoord> list)
    {
        var fromHexOffset = fromHex.ToOffset();
        var toHexOffset = toHex.ToOffset();
        for (int col = Math.Min(fromHexOffset.Column, toHexOffset.Column);
             col <= Math.Max(fromHexOffset.Column, toHexOffset.Column);
             col++)
        {
            for (int row = Math.Min(fromHexOffset.Row, toHexOffset.Row);
                 row <= Math.Max(fromHexOffset.Row, toHexOffset.Row);
                 row++)
            {
                list.Add(new HexOffsetCoord(col, row).ToCube());
            }
        }
    }

    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    static HexCubeCoord LerpRound(HexCubeCoord a, HexCubeCoord b, float t)
    {
        var r = Lerp(a.R, b.R, t);
        var q = Lerp(a.Q, b.Q, t);
        var s = Lerp(a.S, b.S, t);
        return CubeRound(r, q, s);
    }

    public static int Distance(HexCubeCoord a, HexCubeCoord b)
    {
        return (Math.Abs(a.R - b.R) + Math.Abs(a.Q - b.Q) + Math.Abs(a.S - b.S)) / 2;
    }

    public static void LineBetweenHexes(HexCubeCoord a, HexCubeCoord b, ICollection<HexCubeCoord> list)
    {
        var dist = Distance(a, b);
        if (dist == 0)
        {
            list.Add(a);
        }
        else
        {
            for (int i = 0; i <= dist; i++)
            {
                list.Add(LerpRound(a, b, i / (float)dist));
            }
        }
    }

    private static readonly IReadOnlyCollection<HexagonDirection> DirectionOrder = new[]
    {
        HexagonDirection.TopLeft,
        HexagonDirection.Left,
        HexagonDirection.BottomLeft,
        HexagonDirection.BottomRight,
        HexagonDirection.Right,
        HexagonDirection.TopRight
    };
    
    public static IEnumerable<HexCubeCoord> HexagonRing(HexCubeCoord center, int radius)
    {
        if (radius == 0)
        {
            yield return center;
        }
        var hex = center;
        for (int i = 0; i < radius; i++)
        {
            hex += HexagonDirection.Right;
        }

        foreach (var direction in DirectionOrder)
        {
            for (int i = 0; i < radius; i++)
            {
                yield return hex;
                hex += direction;
            }
        }
    }

    public static IEnumerable<HexCubeCoord> HexagonArea(HexCubeCoord center, int radius)
    {
        for (int r = 0; r <= radius; r++)
        {
            foreach (var hex in HexagonRing(center, r))
            {
                yield return hex;
            }
        }
    }
}