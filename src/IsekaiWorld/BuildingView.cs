using System;
using Godot;

public class BuildingView
{
    public EntityMessaging Messaging { get; }
    private readonly GameNode _gameNode;

    public BuildingView(GameNode gameNode)
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
            case BuildingUpdated buildingUpdated:
                OnBuildingUpdated(buildingUpdated);
                break;
            case BuildingRemoved buildingRemoved:
                OnBuildingRemoved(buildingRemoved);
                break;
            case ConstructionUpdated constructionUpdated:
                Execute(constructionUpdated);
                break;
            case ConstructionRemoved constructionRemoved:
                Execute(constructionRemoved);
                break;
        }
    }

    private void OnBuildingUpdated(BuildingUpdated message)
    {
        if (message.Definition.EdgeConnected)
            return;
        
        Node2D buildingNode = _gameNode.MapNode.GetNodeOrNull<Node2D>(message.EntityId);
        if (buildingNode == null)
        {
            buildingNode = new Node2D();
            buildingNode.Name = message.EntityId;
            _gameNode.MapNode.AddChild(buildingNode);

            // Line2D outline = new Line2D();
            // buildingNode.AddChild(outline);
            // outline.Points = new[]{new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1)};
            // outline.Width = 0.1f;
            // outline.DefaultColor = Colors.Red;
            
            buildingNode.Position = EntityCenterPosition(message);

            var sprite = new Sprite();
            buildingNode.AddChild(sprite);

            var textureResource = message.Definition.TextureResource[message.Rotation];
            var texture = ResourceLoader.Load<Texture>(textureResource);
            sprite.Texture = texture;
            sprite.Scale = Vector2.One / texture.GetSize();
            sprite.Modulate = message.Definition.Color;

            if (message.Definition == BuildingDefinitions.WoodenChair)
            {
                sprite.Scale *= 1.3f;
                if (message.Rotation == HexagonDirection.Left)
                {
                    sprite.Scale *= new Vector2(-1, 1);
                }
            }
            
            if (message.Definition == BuildingDefinitions.WoodenBed)
            {
                buildingNode.Scale *= new Vector2(2, 1);
                buildingNode.Rotation = GetRotation(message.Rotation);

                sprite.Scale *= 1.3f;
                sprite.Rotation = GetSpriteRotation(message.Rotation);
                sprite.Scale *= GetBedSpriteScale(message.Rotation);
            }

            if (message.Definition == BuildingDefinitions.TableStoveFueled)
            {
                buildingNode.Scale *= new Vector2(1, 3);
                buildingNode.Rotation = GetRotation(message.Rotation) + Mathf.Pi / 6;

                sprite.Scale *= 0.8f;
                sprite.Rotation = GetSpriteRotation(message.Rotation);
                sprite.Scale *= new Vector2(2, 2);
            }

            if (message.Definition == BuildingDefinitions.TreeOak)
            {
                buildingNode.Scale *= new Vector2(5, 5);
                sprite.Offset = new Vector2(0, -200);
            }
        }
    }

    private Vector2 EntityCenterPosition(BuildingUpdated message)
    {
        if (message.Definition == BuildingDefinitions.WoodenBed)
        {
            var hexA = message.Position;
            var hexB = message.Position + message.Rotation;
            return (hexA.Center(1) + hexB.Center(1)) / 2;
        }
        
        return message.Position.Center(1);
    }

    private float GetRotation(HexagonDirection entityRotation)
    {
        int rotationIndex = (int)entityRotation;
        return rotationIndex * (Mathf.Pi / 3);
    }

    private float GetSpriteRotation(HexagonDirection entityRotation)
    {
        switch (entityRotation)
        {
            case HexagonDirection.Right:
                return 0;
            case HexagonDirection.BottomRight:
                return -Mathf.Pi / 2;
            case HexagonDirection.BottomLeft:
                return -Mathf.Pi / 2;
            case HexagonDirection.Left:
                return 0;
            case HexagonDirection.TopLeft:
                return Mathf.Pi / 2;
            case HexagonDirection.TopRight:
                return Mathf.Pi / 2;
            default:
                throw new ArgumentOutOfRangeException(nameof(entityRotation), entityRotation, null);
        }
    }
    
    private Vector2 GetBedSpriteScale(HexagonDirection entityRotation)
    {
        switch (entityRotation)
        {
            case HexagonDirection.Right:
                return new Vector2(1, 2);
            case HexagonDirection.BottomRight:
                return new Vector2(2, 1);
            case HexagonDirection.BottomLeft:
                return new Vector2(2, 1);
            case HexagonDirection.Left:
                return new Vector2(1, -2);
            case HexagonDirection.TopLeft:
                return new Vector2(2, 1);
            case HexagonDirection.TopRight:
                return new Vector2(2, 1);
            default:
                throw new ArgumentOutOfRangeException(nameof(entityRotation), entityRotation, null);
        }
    }

    private void OnBuildingRemoved(BuildingRemoved message)
    {
        if (message.Definition.EdgeConnected)
            return;
        
        Node2D buildingNode = _gameNode.MapNode.GetNodeOrNull<Node2D>(message.EntityId);
        if (buildingNode != null)
        {
            buildingNode.GetParent().RemoveChild(buildingNode);
        }
    }

    private void Execute(ConstructionUpdated constructionUpdated)
    {
        var nodeName = constructionUpdated.Id;
        var constructioNode = _gameNode.MapNode.GetNodeOrNull<HexagonNode>(nodeName);
        if (constructioNode == null)
        {
            var constructionNode = new HexagonNode
            {
                Name = nodeName, 
                Color = Colors.MediumPurple,
            };
            
            constructionNode.HexPosition = constructionUpdated.Position;
            _gameNode.MapNode.AddChild(constructionNode);
        }
        else
        {
            var percentProgress = constructionUpdated.ProgressRelative;
            constructioNode.InnerSize = Mathf.Min(Mathf.Max((1 - percentProgress)*0.9f, 0), 0.9f);            
        }
    }

    private void Execute(ConstructionRemoved constructionRemoved)
    {
        var constructionNode = _gameNode.MapNode.GetNodeOrNull<HexagonNode>(constructionRemoved.Id);
        _gameNode.MapNode.RemoveChild(constructionNode);
    }
}