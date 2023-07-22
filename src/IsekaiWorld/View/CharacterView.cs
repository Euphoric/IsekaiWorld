using System;
using Godot;
using IsekaiWorld.Game;

namespace IsekaiWorld.View;

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
        if (message.Label == "Anon")
        {
            character.BodyType = "Male";
            character.Clothes = "ShirtBasic/ShirtBasic";
            character.ClothesColor = new Color("CFCFCF");
            character.Hair = "Hair_Male/AFUm03";
            character.HairColor = new Color("BB6D3E");
        }
        else if (message.Label == "Miku")
        {
            character.BodyType = "Female";
            character.Clothes = "ShirtBasic/ShirtBasic";
            character.ClothesColor = new Color("5A676B");
            character.Hair = "Hair_Female/AFUf25";
            character.HairColor = new Color("47C8C0");
        }
        else if (message.Label == "Reimu")
        {
            character.BodyType = "Female";
            character.Clothes = "Blouse/Blouse";
            character.ClothesColor = new Color("FE0000");
            character.Hair = "Hair_Female/AFUf20";
            character.HairColor = new Color("3B1E08");
        }
        else if (message.Label == "Madoka")
        {
            character.BodyType = "Female";
            character.Clothes = "DameDress/DameDress";
            character.ClothesColor = new Color("ffb6bb");
            character.Hair = "Hair_Female/AFUf10";
            character.HairColor = new Color("ffbae4");
        }
        else
        {
            throw new Exception("Unknown character label");
        }

        characterHexagon.AddChild(character);

        var mapNode = _gameNode.EntitiesNode;
        mapNode.AddChild(characterHexagon);
        
        var labelControl = new Node2D();
        labelControl.Name = "LabelControl";
        labelControl.Scale = Vector2.One / 35f;
        characterHexagon.AddChild(labelControl);

        var label = new Label
        {
            Name = "NameLabel",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            Size = new Vector2(200, 80),
            Position = new Vector2(-100, 0)+new Vector2(0, 37),
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            Text = message.Label, // order of setting text matters
        };
        labelControl.AddChild(label);
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