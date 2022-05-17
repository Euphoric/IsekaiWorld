using System;
using Godot;

public readonly struct HexCubeCoord
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
        var q = (Mathf.Sqrt(3) / 3 * position.x - 1f / 3 * position.y) / size;
        var r = (2f / 3 * position.y) / size;
        var s = 0 - r - q;

        return CubeRound(r, q, s);
    }

    public static Vector2 HexCorner(HexCubeCoord coordinates, float size, int i)
    {
        var center = coordinates.Center(size);

        var angleDeg = 60 * i - 30;
        var angleRad = (float)(Math.PI / 180 * angleDeg);
        return new Vector2(center.x + size * Mathf.Cos(angleRad),
            center.y + size * Mathf.Sin(angleRad));
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
}