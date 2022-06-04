using Godot;

public class CreateCharacter : INodeOperation
{
    private readonly CharacterEntity _character;

    public CreateCharacter(CharacterEntity character)
    {
        _character = character;
    }

    public void Execute(HexagonalMap map)
    {
        var characterHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Color = Colors.Blue
        };
        map.AddChild(characterHexagon);
        map.AddNodeReference(_character, characterHexagon);
    }
}