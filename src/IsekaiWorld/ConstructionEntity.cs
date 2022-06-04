public class ConstructionEntity
{
    public HexCubeCoord Position { get; set; }

    public float Progress { get; set; }
    public float CompleteProgress => 3;

    public INodeOperation UpdateNode()
    {
        return new UpdateConstruction(this);
    }
}