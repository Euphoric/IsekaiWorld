public class UpdateCharacter : INodeOperation
{
    private readonly CharacterEntity _characterEntity;

    public UpdateCharacter(CharacterEntity characterEntity)
    {
        _characterEntity = characterEntity;
    }

    public void Execute(GameNode gameNode)
    {
        var node = gameNode.GetEntityNode<HexagonNode>(_characterEntity);
        node.HexPosition = _characterEntity.Position;
    }
}