using System.Collections.Generic;

public class BuildingEntity : IEntity
{
    public BuildingEntity(HexCubeCoord position, BuildingDefinition buildingDefinition)
    {
        Position = position;
        BuildingDefinition = buildingDefinition;
    }

    public HexCubeCoord Position { get; }
    public BuildingDefinition BuildingDefinition { get; }
    public string Label => BuildingDefinition.Label;
    
    public IEnumerable<INodeOperation> Update()
    {
        yield break;
    }
}