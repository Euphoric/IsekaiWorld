using Godot;

public class BuildingDefinition
{
    public string Id { get; }
    public string Label { get; }
    public Color Color { get; }

    public BuildingDefinition(string id, string label, Color color)
    {
        Id = id;
        Label = label;
        Color = color;
    }
}