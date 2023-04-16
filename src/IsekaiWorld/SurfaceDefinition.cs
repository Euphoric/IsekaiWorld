using Godot;

namespace IsekaiWorld;

public class SurfaceDefinition
{
    public string Id { get; }
    public Color Color { get; }
    public string? Texture { get; }
    public bool IsPassable { get; }
    public float TextureScale { get; }

    public SurfaceDefinition(
        string id,
        Color color,
        string? texture,
        float textureScale,
        bool isPassable)
    {
        Id = id;
        Color = color;
        Texture = texture;
        IsPassable = isPassable;
        TextureScale = textureScale;
    }

    public override string ToString()
    {
        return $"[Surface:{Id}]";
    }
}