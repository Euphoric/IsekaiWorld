using Godot;
using IsekaiWorld.Game;

namespace IsekaiWorld.View;

public class MapItemView
{
    public MessagingEndpoint Messaging { get; }
    private readonly GameNode _gameNode;

    public MapItemView(GameNode gameNode)
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
            case ItemUpdated itemUpdated:
                OnItemUpdated(itemUpdated);
                break;
            case ItemPickedUp itemPickedUp:
                RemoveItem(itemPickedUp.EntityId);
                break;
            case ItemRemoved itemRemoved:
                RemoveItem(itemRemoved.EntityId);
                break;
        }
    }

    private void OnItemUpdated(ItemUpdated itemUpdated)
    {
        var mapNode = _gameNode.GetNode<Node2D>("Map");

        var existingNode = mapNode.GetNodeOrNull<HexagonNode>(itemUpdated.EntityId);
        if (existingNode == null)
        {
            var texture = ResourceLoader.Load<Texture2D>(itemUpdated.Definition.TextureResource);
            var size = texture.GetSize();

            var sprite = new Sprite2D
            {
                Position = Vector2.Zero,
                Texture = texture,
                Scale = Vector2.One / size
            };

            var itemNode = new HexagonNode
            {
                Name = itemUpdated.EntityId,
                HexPosition = itemUpdated.Position,
                Color = Colors.Transparent,
                OuterSize = 0f,
                InnerSize = 0f,
            };
            itemNode.AddChild(sprite);
            
            var label = new Label
            {
                Name = "CountLabel",
                Text = itemUpdated.Count.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                Scale = Vector2.One / 25f
            };
            itemNode.AddChild(label);

            mapNode.AddChild(itemNode);
        }
        else
        {
            existingNode.GetNode<Label>("CountLabel").Text = itemUpdated.Count.ToString();
        }
    }

    private void RemoveItem(string itemEntityId)
    {
        var mapNode = _gameNode.GetNode<Node2D>("Map");

        var existingNode = mapNode.GetNodeOrNull<HexagonNode>(itemEntityId);
        existingNode?.GetParent().RemoveChild(existingNode);
    }
}