public class BuildingDefinition
{
    public string Label { get; }
    public SurfaceDefinition Surface { get; }

    public BuildingDefinition(string label, SurfaceDefinition surface)
    {
        Label = label;
        Surface = surface;
    }
}

public static class BuildingDefinitions
{
    public static readonly BuildingDefinition Wall = new BuildingDefinition("Wall", SurfaceDefinitions.ConstructedWall);
}