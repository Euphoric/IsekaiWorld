using System;

namespace IsekaiWorld;

public class ConstructionTestView
{
    public ConstructionTestView(string id)
    {
        Id = id;
    }
    
    public string Id { get; }
    public HexCubeCoord Position { get; private set; }
    public float Progress { get; private set; }
    public ConstructionDefinition Definition { get; private set; } = null!;
    
    public void UpdateFrom(ConstructionUpdated evnt)
    {
        Position = evnt.Position;
        Progress = evnt.ProgressRelative;
        Definition = evnt.Definition;
    }
}