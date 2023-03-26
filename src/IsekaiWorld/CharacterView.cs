using Godot;

public class CharacterView
{
    public MessagingEndpoint Messaging { get; }
    
    private readonly GameNode _gameNode;

    public CharacterView(GameNode gameNode)
    {
        _gameNode = gameNode;
        Messaging = new MessagingEndpoint(MessageHandler);
    }
    
    public void Update()
    {
    }
    
    private void MessageHandler(IEntityMessage message)
    {
        switch (message)
        {
            case CharacterCreated characterCreated:
                OnCharacterCreated(characterCreated);
                break;
            case CharacterUpdated characterUpdated:
                OnCharacterUpdated(characterUpdated);
                break;
        }
    }

    private void OnCharacterCreated(CharacterCreated message)
    {
        var characterHexagon = new HexagonNode
        {
            Name = message.EntityId,
            HexPosition = HexCubeCoord.Zero,
            Color = Colors.Blue
        };
        var mapNode = _gameNode.EntitiesNode;
        mapNode.AddChild(characterHexagon);
    }

    private void OnCharacterUpdated(CharacterUpdated message)
    {
        var node = _gameNode.EntitiesNode.GetNode<HexagonNode>(message.EntityId);
        node.HexPosition = message.Position;
    }
}