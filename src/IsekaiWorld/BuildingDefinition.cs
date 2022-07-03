using System.Collections.Generic;
using Godot;

public class BuildingDefinition
{
    public string Id { get; }
    public string Label { get; }
    public Color Color { get; }
    public bool EdgeConnected { get; }
    public IReadOnlyDictionary<HexagonDirection, string> TextureResource { get; }
    public bool Impassable { get; }
    
    public BuildingDefinition(string id, string label, Color color, Dictionary<HexagonDirection, string> textureResource, bool edgeConnected = false, bool impassable = false)
    {
        Id = id;
        Label = label;
        Color = color;
        TextureResource = textureResource;
        EdgeConnected = edgeConnected;
        Impassable = impassable;
    }
}