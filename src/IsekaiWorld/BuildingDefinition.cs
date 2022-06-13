using Godot;

public class BuildingDefinition
{
    public string Id { get; }
    public string Label { get; }
    public Color Color { get; }
    public bool EdgeConnected { get; }

    public BuildingDefinition(string id, string label, Color color, bool edgeConnected = false)
    {
        Id = id;
        Label = label;
        Color = color;
        EdgeConnected = edgeConnected;
    }
}