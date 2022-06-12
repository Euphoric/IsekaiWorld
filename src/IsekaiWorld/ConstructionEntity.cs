using System.Collections.Generic;

public class ConstructionEntity : IEntity
{
    
    public HexCubeCoord Position { get; }
    public BuildingDefinition BuildingDefinition { get; }

    public ConstructionEntity(HexCubeCoord position, BuildingDefinition buildingDefinition)
    {
        Position = position;
        BuildingDefinition = buildingDefinition;
    }
    
    public float Progress { get; set; }
    public float CompleteProgress => 3;
    public float ProgressRelative => Progress / CompleteProgress;

    public IEnumerable<INodeOperation> Update()
    {
        yield return new UpdateConstruction(this);
    }
}