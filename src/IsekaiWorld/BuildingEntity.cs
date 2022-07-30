using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BuildingEntity : IEntity
{
    public Guid Id { get; }

    private bool _toRemove;
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

    public bool IsRemoved { get; private set; }

    public IEnumerable<INodeOperation> Update()
    {
        if (_toRemove)
        {
            Messaging.Broadcast(new BuildingRemoved(Position, Definition, Id.ToString(), Rotation));
            IsRemoved = true;
        }
        else if (_isDirty)
        {
            Messaging.Broadcast(new BuildingUpdated(Position, Definition, Id.ToString(), Rotation));

            _isDirty = false;
        }
        
        return Enumerable.Empty<INodeOperation>();
    }

    public ItemDefinition ReservedForItem { get; private set; }
    public void ReserveForItem(ItemDefinition item)
    {
        ReservedForItem = item;
    }

    public void RemoveEntity()
    {
        _toRemove = true;
        _isDirty = true;
    }
}

public class BuildingUpdated : IEntityMessage
{
    public BuildingUpdated(HexCubeCoord position, BuildingDefinition definition, string entityId, HexagonDirection rotation)
    {
        Position = position;
        Definition = definition;
        EntityId = entityId;
        Rotation = rotation;
    }
    
    public HexCubeCoord Position { get; }
    public BuildingDefinition Definition { get; }
    public String EntityId { get; }
    public HexagonDirection Rotation { get; }
}

public class BuildingRemoved : IEntityMessage
{
    public BuildingRemoved(HexCubeCoord position, BuildingDefinition definition, string entityId, HexagonDirection rotation)
    {
        Position = position;
        Definition = definition;
        EntityId = entityId;
        Rotation = rotation;
    }
    
    public HexCubeCoord Position { get; }
    public BuildingDefinition Definition { get; }
    public String EntityId { get; }
    public HexagonDirection Rotation { get; }
}