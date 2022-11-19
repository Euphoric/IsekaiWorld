using Godot;

public class CharacterView
{
    public EntityMessaging Messaging { get; }
    
    private readonly GameNode _gameNode;

    public CharacterView(GameNode gameNode)
    {
        _gameNode = gameNode;
        Messaging = new EntityMessaging();
    }
    
    public void Update()
    {
        Messaging.HandleMessages(MessageHandler);
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
        var mapNode = _gameNode.MapNode;
        mapNode.AddChild(characterHexagon);
    }

    private void OnCharacterUpdated(CharacterUpdated message)
    {
        var node = _gameNode.MapNode.GetNode<HexagonNode>(message.EntityId);
        node.HexPosition = message.Position;
    }
}