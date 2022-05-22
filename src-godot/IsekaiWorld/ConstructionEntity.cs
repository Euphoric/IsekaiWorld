using Godot;

public class ConstructionEntity
{
    public HexCubeCoord Position { get; set; }

    public float Progress { get; set; }
    public float CompleteProgress => 3;

    public HexagonNode Node { get; set; }

    public void UpdateNode()
    {
        var percentProgress = Progress / CompleteProgress;
        Node.InnerSize = Mathf.Min(Mathf.Max((1 - percentProgress)*0.9f, 0), 0.9f);
    }
}