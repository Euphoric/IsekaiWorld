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
            Name = _character.Id.ToString(),
            HexPosition = HexCubeCoord.Zero,
            Color = Colors.Blue
        };
        var mapNode = gameNode.MapNode;
        mapNode.AddChild(characterHexagon);
    }
}