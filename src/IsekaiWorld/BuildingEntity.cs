using System;
using System.Collections.Generic;
using Godot;

public class BuildingEntity : IEntity
{
    public Guid Id { get; }
    
    private bool _isDirty;

    public BuildingEntity(HexCubeCoord position, BuildingDefinition definition)
    {
        Id = Guid.NewGuid();
        
        Position = position;
        Definition = definition;
        _isDirty = true;
    }

    public HexCubeCoord Position { get; }
    public BuildingDefinition Definition { get; }
    public string Label => Definition.Label;

    public bool IsRemoved => false;

    public IEnumerable<INodeOperation> Update()
    {
        if (_isDirty)
        {
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
        var buildingNode = gameNode.MapNode.GetNodeOrNull<HexagonNode>(Entity.Id.ToString());

        if (buildingNode == null)
        {
            buildingNode = new HexagonNode();
            gameNode.MapNode.AddChild(buildingNode);

            buildingNode.HexPosition = Entity.Position;
            buildingNode.Color = Colors.Transparent;
            buildingNode.InnerSize = 0.0f;
            buildingNode.OuterSize = 0.0f;

            var sprite = new Sprite();
            buildingNode.AddChild(sprite);

            var texture = ResourceLoader.Load<Texture>(Entity.Definition.TextureResource);
            sprite.Texture = texture;
            sprite.Scale = Vector2.One / texture.GetSize();
            sprite.Modulate = Entity.Definition.Color;
        }
    }
}