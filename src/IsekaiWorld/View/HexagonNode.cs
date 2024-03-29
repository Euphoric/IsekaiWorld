using Godot;
using IsekaiWorld.Game;

namespace IsekaiWorld.View;

public partial class HexagonNode : Node2D
{
    private HexCubeCoord _hexPosition;
    private ArrayMesh _hexesMesh = null!;
    private Color _color;
    private bool _isDirty;
    private float _outerSize = 1f;
    private float _innerSize = 0.9f;

    public HexCubeCoord HexPosition
    {
        get => _hexPosition;
        set
        {
            _hexPosition = value;
            Position = _hexPosition.Center(1);
        }
    }

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            _isDirty = true;
        }
    }

    public float OuterSize
    {
        get => _outerSize;
        set
        {
            _outerSize = value;
            _isDirty = true;
        }
    }

    public float InnerSize
    {
        get => _innerSize;
        set
        {
            _innerSize = value;
            _isDirty = true;
        }
    }

    public override void _Ready()
    {
        _hexesMesh = new ArrayMesh();
        
        RebuildMesh();
    }

    public override void _Process(double delta)
    {
        if (_isDirty)
            RebuildMesh();
    }

    private void RebuildMesh()
    {
        _hexesMesh.ClearSurfaces();

        var outerSize = OuterSize;
        var innerSize = InnerSize;

        Vector2[] points = new Vector2[12];
        for (int i = 0; i < 6; i++)
        {
            points[i * 2 + 0] = HexCubeCoord.HexCorner(HexCubeCoord.Zero, outerSize, i);
            points[i * 2 + 1] = HexCubeCoord.HexCorner(HexCubeCoord.Zero, innerSize, i);
        }

        var hexColor = Color;

        Color[] colors =
        {
            hexColor, hexColor,
            hexColor, hexColor,
            hexColor, hexColor,
            hexColor, hexColor,
            hexColor, hexColor,
            hexColor, hexColor
        };
        var indices = new[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1
        };
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = Variant.CreateFrom(points);
        arrays[(int)Mesh.ArrayType.Index] = Variant.CreateFrom(indices);
        arrays[(int)Mesh.ArrayType.Color] = Variant.CreateFrom(colors);
        _hexesMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleStrip, arrays);
        
        _isDirty = false;
    }

    public override void _Draw()
    {
        DrawMesh(_hexesMesh, null);
    }
}