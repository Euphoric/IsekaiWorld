public class UpdateCharacter : INodeOperation
{
    private readonly CharacterEntity _characterEntity;

    public UpdateCharacter(CharacterEntity characterEntity)
    {
        _characterEntity = characterEntity;
    }

    public void Execute(HexagonalMap map)
    {
        var node = map.GetEntityNode<HexagonNode>(_characterEntity);
        node.HexPosition = _characterEntity.Position;
    }
}