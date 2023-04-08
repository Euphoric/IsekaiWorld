using Godot;

namespace IsekaiWorld;

public class BuildingTestView
{
    public BuildingTestView(string id)
    {
        Id = id;
    }
    
    public string Id { get; }
    public HexCubeCoord Position { get; private set; }
    public BuildingDefinition Definition { get; private set; } = null!;
    public DesignationDefinition? Designation { get; private set; }

    public void UpdateFrom(BuildingUpdated evnt)
    {
        Position = evnt.Position;
        Definition = evnt.Definition;
        Designation = evnt.Designation;
    }
}