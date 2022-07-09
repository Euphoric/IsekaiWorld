using Godot;

public class UpdateItemOperation : INodeOperation
{
    public ItemEntity ItemEntity { get; }

    public UpdateItemOperation(ItemEntity itemEntity)
    {
        ItemEntity = itemEntity;
    }

    public void Execute(GameNode gameNode)
    {
        var mapNode = gameNode.GetNode<Node2D>("Map");

        var existingNode = mapNode.GetNodeOrNull<HexagonNode>(ItemEntity.EntityId.ToString());
        if (existingNode == null)
        {
            var texture = ResourceLoader.Load<Texture>(ItemEntity.Definition.TextureResource);
            var size = texture.GetSize();

            var sprite = new Sprite
            {
                Position = Vector2.Zero,
                Texture = texture,
                Scale = Vector2.One / size
            };

            var label = new Label();
            label.Name = "CountLabel";
            label.Text = ItemEntity.Count.ToString();
            label.Align = Label.AlignEnum.Center;
            label.RectScale = Vector2.One / 25f;

            var itemNode = new HexagonNode
            {
                Name = ItemEntity.EntityId.ToString(),
                HexPosition = ItemEntity.Position,
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
            existingNode.GetNode<Label>("CountLabel").Text = ItemEntity.Count.ToString();
        }
    }
}

public class RemoveItemOperation : INodeOperation
{
    public ItemEntity ItemEntity { get; }

    public RemoveItemOperation(ItemEntity itemEntity)
    {
        ItemEntity = itemEntity;
    }

    public void Execute(GameNode gameNode)
    {
        var mapNode = gameNode.GetNode<Node2D>("Map");

        var existingNode = mapNode.GetNodeOrNull<HexagonNode>(ItemEntity.EntityId.ToString());
        existingNode?.GetParent().RemoveChild(existingNode);
    }
}