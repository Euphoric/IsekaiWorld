using Godot;

public class UpdateConstruction : INodeOperation
{
    private readonly ConstructionEntity _constructionEntity;

    public UpdateConstruction(ConstructionEntity constructionEntity)
    {
        _constructionEntity = constructionEntity;
    }

    public void Execute(GameNode gameNode)
    {
        var node = gameNode.GetEntityNode<HexagonNode>(_constructionEntity);
        
        var percentProgress = _constructionEntity.ProgressRelative;
        node.InnerSize = Mathf.Min(Mathf.Max((1 - percentProgress)*0.9f, 0), 0.9f);
    }
}