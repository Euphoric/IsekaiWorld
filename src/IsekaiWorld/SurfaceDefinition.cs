using Godot;

namespace IsekaiWorld;

public class SurfaceDefinition
{
    public string Id { get; }
    public Color Color { get; }
    public bool IsPassable { get; }

    public SurfaceDefinition(string id, Color color, bool isPassable)
    {
        Id = id;
        Color = color;
        IsPassable = isPassable;
    }
}