public class UpdateCharacter : INodeOperation
{
    private readonly CharacterEntity _characterEntity;

    public UpdateCharacter(CharacterEntity characterEntity)
    {
        _characterEntity = characterEntity;
    }

    public void Execute(GameNode gameNode)
    {
        var node = gameNode.MapNode.GetNode<HexagonNode>(_characterEntity.Id.ToString());
        node.HexPosition = _characterEntity.Position;
    }
}