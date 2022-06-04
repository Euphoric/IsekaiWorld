using Godot;

public class AddConstructionOperation : INodeOperation
{
    private readonly ConstructionEntity _constructionEntity;

    public AddConstructionOperation(ConstructionEntity constructionEntity)
    {
        _constructionEntity = constructionEntity;
    }

    public void Execute(HexagonalMap map)
    {
        var constructionNode = new HexagonNode
        {
            Color = Colors.MediumPurple,
        };
        constructionNode.HexPosition = _constructionEntity.Position;
        map.AddChild(constructionNode);
        map.AddNodeReference(_constructionEntity, constructionNode);
    }
}