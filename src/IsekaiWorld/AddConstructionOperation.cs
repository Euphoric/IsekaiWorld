using Godot;

public class AddConstructionOperation : INodeOperation
{
    private readonly ConstructionEntity _constructionEntity;

    public AddConstructionOperation(ConstructionEntity constructionEntity)
    {
        _constructionEntity = constructionEntity;
    }

    public void Execute(GameNode gameNode)
    {
        var constructionNode = new HexagonNode
        {
            Color = Colors.MediumPurple,
        };
        constructionNode.HexPosition = _constructionEntity.Position;
        var mapNode = gameNode.GetEntityNode<HexagonalMap>(gameNode.GameEntity.GameMap);
        mapNode.AddChild(constructionNode);
        gameNode.AddNodeReference(_constructionEntity, constructionNode);
    }
}