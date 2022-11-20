using System;
using Godot;

public class MapItemView
{
    public MessagingEndpoint Messaging { get; }
    private readonly GameNode _gameNode;

    public MapItemView(GameNode gameNode)
    {
        _gameNode = gameNode;
        Messaging = new MessagingEndpoint();
    }

    public void Update()
    {
        Messaging.HandleMessages(MessageHandler);
    }

    private void MessageHandler(IEntityMessage message)
    {
        switch (message)
        {
            case ItemUpdated itemUpdated:
                OnItemUpdated(itemUpdated);
                break;
            case ItemPickedUp itemPickedUp:
                OnItemPickedUp(itemPickedUp);
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

            var label = new Label();
            label.Name = "CountLabel";
            label.Text = itemUpdated.Count.ToString();
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Scale = Vector2.One / 25f;

            var itemNode = new HexagonNode
            {
                Name = itemUpdated.EntityId,
                HexPosition = itemUpdated.Position,
                Color = Colors.Transparent,
                OuterSize = 0f,
                InnerSize = 0f,
            };
            itemNode.AddChild(sprite);
            itemNode.AddChild(label);

            mapNode.AddChild(itemNode);
        }
        else
        {
            existingNode.GetNode<Label>("CountLabel").Text = itemUpdated.Count.ToString();
        }
    }

    private void OnItemPickedUp(ItemPickedUp itemPickedUp)
    {
        var mapNode = _gameNode.GetNode<Node2D>("Map");

        var existingNode = mapNode.GetNodeOrNull<HexagonNode>(itemPickedUp.EntityId);
        existingNode?.GetParent().RemoveChild(existingNode);
    }
}