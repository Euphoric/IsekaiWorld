using Godot;

public class CharacterNode : Node2D
{
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

    public CharacterNode()
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

        var polygonNode = new Line2D
        {
            Points = polygon,
            Width = 1.5f / 16
        };
        AddChild(polygonNode);
    }
}