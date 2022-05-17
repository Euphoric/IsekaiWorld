using System;
using System.Diagnostics;
using Godot;

public class HexagonalMap : Node2D
{
    private HexMap _map;
    private ArrayMesh _hexesMesh;
    
    private readonly float _hexSize = 16;
    private HexagonNode _mouseoverHexagon;
    private HexagonNode _characterHexagon;
    private HexagonNode _targetHexagon;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _map = new HexMap(32);
        _map.GenerateMap();
        
        _hexesMesh = new ArrayMesh();

        foreach (var cell in _map.Cells)
        {
            var center = cell.Position.Center(_hexSize);
            Vector2[] points = new Vector2[7];
            points[0] = center;
            for (int i = 0; i < 6; i++)
            {
                points[i + 1] = HexCubeCoord.HexCorner(cell.Position, _hexSize, i);
            }

            var hexColor = cell.Surface == 1
                ? Colors.DarkGreen
                : Colors.SaddleBrown;

            Color[] colors = new[]
            {
                hexColor,
                hexColor,
                hexColor,
                hexColor,
                hexColor,
                hexColor,
                hexColor,
            };
            var indices = new int[]
            {
                0, 1, 2, 3, 4, 5, 6, 1
            };
            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)ArrayMesh.ArrayType.Max);
            arrays[(int)ArrayMesh.ArrayType.Vertex] = points;
            arrays[(int)ArrayMesh.ArrayType.Index] = indices;
            arrays[(int)ArrayMesh.ArrayType.Color] = colors;
            _hexesMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleFan, arrays);
        }

        _targetHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Scale = new Vector2(_hexSize, _hexSize),
            Color = Colors.White
        };
        AddChild(_targetHexagon);
        
        _characterHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Scale = new Vector2(_hexSize, _hexSize),
            Color = Colors.Blue
        };
        AddChild(_characterHexagon);

        _mouseoverHexagon = new HexagonNode
        {
            Scale = new Vector2(_hexSize, _hexSize),
            Color = Colors.Red
        };
        AddChild(_mouseoverHexagon);
    }

    public override void _Draw()
    {
        DrawMesh(_hexesMesh, null);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
                _targetHexagon.HexPosition = _mouseoverHexagon.HexPosition;
            }
        }
        
        base._Input(@event);
    }

    public override void _Process(float delta)
    {
        var position = GetLocalMousePosition();

        var size = _hexSize;
        
        var hex = HexCubeCoord.FromPosition(position, size);
        
        _mouseoverHexagon.HexPosition = hex;
        
        base._Process(delta);
    }
}