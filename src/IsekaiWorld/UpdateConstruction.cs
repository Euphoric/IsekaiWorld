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
        var nodeName = _constructionEntity.Id.ToString();
        var constructioNode = gameNode.MapNode.GetNodeOrNull<HexagonNode>(nodeName);
        if (constructioNode == null)
        {
            var constructionNode = new HexagonNode
            {
                Name = nodeName, 
                Color = Colors.MediumPurple,
            };
            
            constructionNode.HexPosition = _constructionEntity.Position;
            gameNode.MapNode.AddChild(constructionNode);
        }
        else
        {
            var percentProgress = _constructionEntity.ProgressRelative;
            constructioNode.InnerSize = Mathf.Min(Mathf.Max((1 - percentProgress)*0.9f, 0), 0.9f);            
        }
    }
}

public class RemoveConstruction : INodeOperation
{
    private readonly ConstructionEntity _construction;

    public RemoveConstruction(ConstructionEntity construction)
    {
        _construction = construction;
    }

    public void Execute(GameNode gameNode)
    {
        var constructionNode = gameNode.MapNode.GetNodeOrNull<HexagonNode>(_construction.Id.ToString());
        gameNode.MapNode.RemoveChild(constructionNode);
    }
}