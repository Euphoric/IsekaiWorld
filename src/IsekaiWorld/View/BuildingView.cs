using System;
using Godot;
using IsekaiWorld.Game;
using Vector2 = Godot.Vector2;

namespace IsekaiWorld.View;

public class BuildingView
{
    private readonly GameNode _gameNode;
    public MessagingEndpoint Messaging { get; }

    private Node2D EntitiesNode => _gameNode.EntitiesNode;
    
    public BuildingView(GameNode gameNode)
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
            case BuildingUpdated buildingUpdated:
                OnBuildingUpdated(buildingUpdated);
                break;
            case BuildingRemoved buildingRemoved:
                OnBuildingRemoved(buildingRemoved);
                break;
            case ConstructionUpdated constructionUpdated:
                OnConstructionUpdated(constructionUpdated);
                break;
            case ConstructionRemoved constructionRemoved:
                OnConstructionRemoved(constructionRemoved);
                break;
        }
    }

    // TODO: Consider non-random based on position
    private readonly Random _rand = new(1325);

    private Vector2 RandomPointInsideCircle()
    {
        Vector2 point;
        do
        {
            point = new Vector2(_rand.NextSingle() * 2 - 1, _rand.NextSingle() * 2 - 1);
        } while (point.DistanceTo(Vector2.Zero) > 1);
        
        var x = point.DistanceTo(Vector2.Zero);
        
        return point;
    }
    
    private void OnBuildingUpdated(BuildingUpdated message)
    {
        if (message.Definition.EdgeConnected)
            return;
        
        Node2D buildingNode = EntitiesNode.GetNodeOrNull<Node2D>(message.EntityId);
        if (buildingNode == null)
        {
            buildingNode = new Node2D();
            buildingNode.Name = message.EntityId;
            EntitiesNode.AddChild(buildingNode);
            
            // Line2D outline = new Line2D();
            // buildingNode.AddChild(outline);
            // outline.Points = new[]{new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1)};
            // outline.Width = 0.1f;
            // outline.DefaultColor = Colors.Red;
            
            buildingNode.Position = EntityCenterPosition(message);

            var multiTextureRendering = 
                message.Definition == BuildingDefinitions.Plant.Grass || 
                message.Definition == BuildingDefinitions.Plant.WildRice
                ;

            if (multiTextureRendering)
            {
                var pieceCount = _rand.Next(3, 7);
                for (int i = 0; i < pieceCount; i++)
                {
                    var sprite = new Sprite2D();
                    buildingNode.AddChild(sprite);

                    var textureResource = message.Definition.TextureResource[message.Rotation];
                    var texture = ResourceLoader.Load<Texture2D>(textureResource);
                    sprite.Texture = texture;
                    sprite.Modulate = message.Definition.Color;

                    sprite.Position = RandomPointInsideCircle() * 0.8f;
                
                    if (message.Definition == BuildingDefinitions.Plant.Grass)
                    {
                        // TODO Use both grass textures
                        
                        var randomSize = Mathf.Lerp(0.8f, 1.2f, _rand.NextSingle());
                        var plantSize = 1.0f * randomSize;
                        sprite.Scale = Vector2.One / texture.GetSize() * plantSize;
                    }

                    if (message.Definition == BuildingDefinitions.Plant.WildRice)
                    {
                        var randomSize = Mathf.Lerp(0.8f, 1.2f, _rand.NextSingle());
                        var plantSize = 1.0f * randomSize;
                        sprite.Scale = Vector2.One / texture.GetSize() * plantSize;
                    }
                }
            }
            else
            {

                var sprite = new Sprite2D();
                buildingNode.AddChild(sprite);

                var textureResource = message.Definition.TextureResource[message.Rotation];
                var texture = ResourceLoader.Load<Texture2D>(textureResource);
                sprite.Texture = texture;
                sprite.Modulate = message.Definition.Color;

                if (message.Definition == BuildingDefinitions.WoodenChair)
                {
                    sprite.Scale = Vector2.One / texture.GetSize();
                    sprite.Scale *= 1.3f;
                    sprite.Scale *= new Vector2(1, texture.GetHeight() / (float)texture.GetWidth());
                    if (message.Rotation == HexagonDirection.Left || message.Rotation == HexagonDirection.BottomLeft ||
                        message.Rotation == HexagonDirection.TopLeft)
                    {
                        sprite.Scale *= new Vector2(-1, 1);
                    }
                }

                if (message.Definition == BuildingDefinitions.WoodenBed)
                {
                    sprite.Scale = Vector2.One / texture.GetSize();
                    sprite.Scale *= 1.3f;
                    sprite.Scale *= 2;
                    sprite.Scale *= new Vector2(1, texture.GetHeight() / (float)texture.GetWidth());
                    if (message.Rotation == HexagonDirection.Left || message.Rotation == HexagonDirection.BottomLeft ||
                        message.Rotation == HexagonDirection.TopLeft)
                    {
                        sprite.Rotation = 0;
                        sprite.Scale *= new Vector2(-1, 1);
                    }
                }

                if (message.Definition == BuildingDefinitions.TableStoveFueled)
                {
                    sprite.Scale = Vector2.One / texture.GetSize();

                    buildingNode.Scale *= new Vector2(1, 3);
                    buildingNode.Rotation = GetRotation(message.Rotation) + Mathf.Pi / 6;

                    sprite.Scale *= 0.8f;
                    sprite.Rotation = GetSpriteRotation(message.Rotation);
                    sprite.Scale *= new Vector2(2, 2);
                }

                if (message.Definition == BuildingDefinitions.Plant.TreeOak)
                {
                    var randomSize = Mathf.Lerp(0.8f, 1.2f, _rand.NextSingle());
                    var plantSize = 5 * randomSize;
                    sprite.Scale = Vector2.One / texture.GetSize() * plantSize;
                    sprite.Offset = new Vector2(0, -200);
                }

                if (message.Definition == BuildingDefinitions.CraftingDesk)
                {
                    var spriteScale = new Vector2(64 * Mathf.Sqrt(3) * 2, 64 * 2);
                    var textureScale = 4;

                    var spriteInTextureScale = spriteScale * textureScale;

                    sprite.Scale = (new Vector2(1 * Mathf.Sqrt(3), 1) * 2) / spriteInTextureScale;
                }
                
                if (message.Definition == BuildingDefinitions.CombatDummy)
                {
                    sprite.Scale = Vector2.One / texture.GetSize();
                    sprite.Scale *= 1.3f;
                    sprite.Scale *= new Vector2(1, texture.GetHeight() / (float)texture.GetWidth());
                }
            }
        }

        var designation = message.Designation;
        if (designation != null)
        {
            var designationNodeName = "Designation-" + designation.Id.Replace('.', '-');
            var existingDesignationNode = buildingNode.GetNodeOrNull<Sprite2D>(designationNodeName);
            if (existingDesignationNode == null)
            {
                var designationNode = new Sprite2D();
                designationNode.Name = designationNodeName;
                var texture = ResourceLoader.Load<Texture2D>(designation.TexturePath);
                designationNode.Texture = texture;
                designationNode.Scale = Vector2.One / texture.GetSize();
                buildingNode.AddChild(designationNode);
            }
        }
    }

    private Vector2 EntityCenterPosition(BuildingUpdated message)
    {
        if (message.Definition == BuildingDefinitions.WoodenBed || message.Definition == BuildingDefinitions.CraftingDesk)
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

    private void OnBuildingRemoved(BuildingRemoved message)
    {
        if (message.Definition.EdgeConnected)
            return;
        
        Node2D buildingNode = EntitiesNode.GetNodeOrNull<Node2D>(message.EntityId);
        if (buildingNode != null)
        {
            buildingNode.GetParent().RemoveChild(buildingNode);
        }
    }

    private void OnConstructionUpdated(ConstructionUpdated constructionUpdated)
    {
        var nodeName = constructionUpdated.EntityId;
        var constructioNode = EntitiesNode.GetNodeOrNull<HexagonNode>(nodeName);
        if (constructioNode == null)
        {
            var constructionNode = new HexagonNode
            {
                Name = nodeName, 
                Color = Colors.MediumPurple,
            };
            
            constructionNode.HexPosition = constructionUpdated.Position;
            EntitiesNode.AddChild(constructionNode);
        }
        else
        {
            var percentProgress = constructionUpdated.ProgressRelative;
            constructioNode.InnerSize = Mathf.Min(Mathf.Max((1 - percentProgress)*0.9f, 0), 0.9f);            
        }
    }

    private void OnConstructionRemoved(ConstructionRemoved constructionRemoved)
    {
        var constructionNode = EntitiesNode.GetNodeOrNull<HexagonNode>(constructionRemoved.EntityId);
        EntitiesNode.RemoveChild(constructionNode);
    }
}