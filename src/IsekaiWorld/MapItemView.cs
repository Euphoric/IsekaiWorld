using System;
using Godot;

public class MapItemView
{
    public EntityMessaging Messaging { get; }
    private readonly GameNode _gameNode;

    public MapItemView(GameNode gameNode)
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
            var texture = ResourceLoader.Load<Texture>(itemUpdated.Definition.TextureResource);
            var size = texture.GetSize();

            var sprite = new Sprite
            {
                Position = Vector2.Zero,
                Texture = texture,
                Scale = Vector2.One / size
            };

            var label = new Label();
            label.Name = "CountLabel";
            label.Text = itemUpdated.Count.ToString();
            label.Align = Label.AlignEnum.Center;
            label.RectScale = Vector2.One / 25f;

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