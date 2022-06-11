
using System.Collections.Generic;

public class BuildingDefinition
{
    public string Id { get; }
    public string Label { get; }
    public SurfaceDefinition Surface { get; }

    public BuildingDefinition(string id, string label, SurfaceDefinition surface)
    {
        Id = id;
        Label = label;
        Surface = surface;
    }
}

public static class BuildingDefinitions
{
    public static readonly BuildingDefinition Wall = new BuildingDefinition("Core.Wall", "Wall", SurfaceDefinitions.ConstructedWall);

    private static readonly Dictionary<string, BuildingDefinition> Definitions = new Dictionary<string, BuildingDefinition>()
    {
        { Wall.Id, Wall }
    };
    
    public static BuildingDefinition GetById(string buildingDefinitionId)
    {
        return Definitions[buildingDefinitionId];
    }
}