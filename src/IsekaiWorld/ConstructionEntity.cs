public class ConstructionEntity
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
    

    public INodeOperation UpdateNode()
    {
        return new UpdateConstruction(this);
    }
}