using Godot;

public class CreateCharacter : INodeOperation
{
    private readonly CharacterEntity _character;

    public CreateCharacter(CharacterEntity character)
    {
        _character = character;
    }

    public void Execute(GameNode gameNode)
    {
        var characterHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Color = Colors.Blue
        };
        var mapNode = gameNode.GetEntityNode<HexagonalMap>(gameNode.GameEntity.GameMap);
        mapNode.AddChild(characterHexagon);
        gameNode.AddNodeReference(_character, characterHexagon);
    }
}