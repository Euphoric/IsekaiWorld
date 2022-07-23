using System;
using System.Collections.Generic;
using Godot;

public class BuildingEntity : IEntity
{
    public Guid Id { get; }
    
    private bool _isDirty;

    public EntityMessaging Messaging { get; } = new EntityMessaging();
    
    public BuildingEntity(HexCubeCoord position, HexagonDirection rotation, BuildingDefinition definition)
    {
        Id = Guid.NewGuid();
        
        Position = position;
        Rotation = rotation;
        
        Definition = definition;
        _isDirty = true;
    }

    public HexCubeCoord Position { get; }
    public ISet<HexCubeCoord> OccupiedCells => new HashSet<HexCubeCoord> { Position };
    public BuildingDefinition Definition { get; }
    public string Label => Definition.Label;

    public HexagonDirection Rotation { get; }
    
    public bool IsRemoved => false;
    public ItemDefinition ReservedForItem { get; private set; }

    public IEnumerable<INodeOperation> Update()
    {
        if (_isDirty)
        {
            Messaging.Broadcast(new BuildingUpdated(Position, Definition));

            if (Definition.EdgeConnected)
            {
                yield return new HexagonalMapEntity.RefreshMapOperation();                
            }
            else
            {
                yield return new UpdateBuildingOperation(this);
            }

            _isDirty = false;
        }
    }

    public void ReserveForItem(ItemDefinition item)
    {
        ReservedForItem = item;
    }
}

public class BuildingUpdated : IEntityMessage
{
    public BuildingUpdated(HexCubeCoord position, BuildingDefinition definition)
    {
        Position = position;
        Definition = definition;
    }
    
    public HexCubeCoord Position { get; }
    public BuildingDefinition Definition { get; }
}

public class UpdateBuildingOperation : INodeOperation
{
    public BuildingEntity Entity { get; }

    public UpdateBuildingOperation(BuildingEntity entity)
    {
        Entity = entity;
    }

    public void Execute(GameNode gameNode)
    {
        Node2D buildingNode = gameNode.MapNode.GetNodeOrNull<Node2D>(Entity.Id.ToString());

        if (buildingNode == null)
        {
            buildingNode = new Node2D();
            gameNode.MapNode.AddChild(buildingNode);

            // Line2D outline = new Line2D();
            // buildingNode.AddChild(outline);
            // outline.Points = new[]{new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1)};
            // outline.Width = 0.1f;
            // outline.DefaultColor = Colors.Red;
            
            buildingNode.Position = EntityCenterPosition();

            var sprite = new Sprite();
            buildingNode.AddChild(sprite);

            var textureResource = Entity.Definition.TextureResource[Entity.Rotation];
            var texture = ResourceLoader.Load<Texture>(textureResource);
            sprite.Texture = texture;
            sprite.Scale = Vector2.One / texture.GetSize();
            sprite.Modulate = Entity.Definition.Color;

            if (Entity.Definition == BuildingDefinitions.WoodenChair)
            {
                sprite.Scale *= 1.3f;
                if (Entity.Rotation == HexagonDirection.Left)
                {
                    sprite.Scale *= new Vector2(-1, 1);
                }
            }
            
            if (Entity.Definition == BuildingDefinitions.WoodenBed)
            {
                buildingNode.Scale *= new Vector2(2, 1);
                buildingNode.Rotation = GetRotation(Entity.Rotation);

                sprite.Scale *= 1.3f;
                sprite.Rotation = GetSpriteRotation(Entity.Rotation);
                sprite.Scale *= GetBedSpriteScale(Entity.Rotation);
            }

            if (Entity.Definition == BuildingDefinitions.TableStoveFueled)
            {
                buildingNode.Scale *= new Vector2(1, 3);
                buildingNode.Rotation = GetRotation(Entity.Rotation) + Mathf.Pi / 6;

                sprite.Scale *= 0.8f;
                sprite.Rotation = GetSpriteRotation(Entity.Rotation);
                sprite.Scale *= new Vector2(2, 2);
            }
        }
    }

    private Vector2 EntityCenterPosition()
    {
        if (Entity.Definition == BuildingDefinitions.WoodenBed)
        {
            var hexA = Entity.Position;
            var hexB = Entity.Position + Entity.Rotation;
            return (hexA.Center(1) + hexB.Center(1)) / 2;
        }
        
        return Entity.Position.Center(1);
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
}