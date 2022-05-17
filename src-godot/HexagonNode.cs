using Godot;

public class HexagonNode : Node2D
{
    private readonly Line2D _polygonNode;
    private HexCubeCoord _hexPosition;

    public HexCubeCoord HexPosition
    {
        get => _hexPosition;
        set
        {
            _hexPosition = value;
            Position = _hexPosition.Center(16);
        }
    }

    public Color Color
    {
        get => _polygonNode.DefaultColor;
        set => _polygonNode.DefaultColor = value;
    }

    public HexagonNode()
    {
        var size = 0.95f;
        var polygon = new[]
        {
            HexCubeCoord.HexCorner(HexCubeCoord.Zero, size, 0),
            HexCubeCoord.HexCorner(HexCubeCoord.Zero, size, 1),
            HexCubeCoord.HexCorner(HexCubeCoord.Zero, size, 2),
            HexCubeCoord.HexCorner(HexCubeCoord.Zero, size, 3),
            HexCubeCoord.HexCorner(HexCubeCoord.Zero, size, 4),
            HexCubeCoord.HexCorner(HexCubeCoord.Zero, size, 5),
            HexCubeCoord.HexCorner(HexCubeCoord.Zero, size, 6)
        };

        _polygonNode = new Line2D
        {
            Points = polygon,
            Width = 1.5f / 16,
            DefaultColor = Colors.Red
        };
        AddChild(_polygonNode);
    }
}