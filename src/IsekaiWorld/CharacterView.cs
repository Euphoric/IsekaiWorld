using System;
using Godot;

namespace IsekaiWorld;

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
            Color = Colors.Transparent
        };
        CharacterNode character = new CharacterNode();
        character.Name = "CharacterSpriteNode";
        character.Scale = Vector2.One * 0.017f;
        character.BodyType = "Female";
        character.Clothes = "DameDress/DameDress";
        character.Hair = "Hair_Female/AFUf08";
        character.HairColor = new Color("FAF0BE");
        characterHexagon.AddChild(character);
        
        var mapNode = _gameNode.EntitiesNode;
        mapNode.AddChild(characterHexagon);
    }

    private void OnCharacterUpdated(CharacterUpdated message)
    {
        var node = _gameNode.EntitiesNode.GetNode<HexagonNode>(message.EntityId);
        node.HexPosition = message.Position;
        var cn = node.GetNode<CharacterNode>("CharacterSpriteNode")!;
        cn.Direction = ToCharacterDirection(message.FacingDirection);
    }

    private string ToCharacterDirection(HexagonDirection direction)
    {
        switch (direction)
        {
            case HexagonDirection.Right:
                return "east";
            case HexagonDirection.BottomRight:
                return "south";
            case HexagonDirection.BottomLeft:
                return "south";
            case HexagonDirection.Left:
                return "west";
            case HexagonDirection.TopLeft:
                return "north";
            case HexagonDirection.TopRight:
                return "north";
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}