using Godot;

public class SurfaceDefinition
{
    public int Id { get; }
    public Color Color { get; }
    public bool IsPassable { get; }

    public SurfaceDefinition(int id, Color color, bool isPassable)
    {
        Id = id;
        Color = color;
        IsPassable = isPassable;
    }
}