using System;
using System.Collections.Generic;

namespace IsekaiWorld;

public class BuildingEntity : IEntity
{
    public Guid Id { get; }

    private bool _toRemove;
    private bool _isDirty;

    public MessagingEndpoint Messaging { get; } = new();
    
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

    public HexagonDirection Rotation { get; }

    public bool IsRemoved { get; private set; }

    public void Initialize()
    {
        Messaging.Broadcast(new BuildingUpdated(Position, Definition, Id.ToString(), Rotation, Designation));
    }
    
    public void Update()
    {
        if (_toRemove)
        {
            Messaging.Broadcast(new BuildingRemoved(Position, Definition, Id.ToString(), Rotation));
            IsRemoved = true;
        }
        else if (_isDirty)
        {
            Messaging.Broadcast(new BuildingUpdated(Position, Definition, Id.ToString(), Rotation, Designation));
            _isDirty = false;
        }
    }
    
    public DesignationDefinition? Designation { get; set; }
    
    public bool ReservedForActivity { get; set; }
    
    public void Remove()
    {
        _toRemove = true;
        _isDirty = true;
    }

    public void TryDesignate(DesignationDefinition designation)
    {
        if (!Definition.AllowedDesignations.Contains(designation)) 
            return;
        
        Designation = designation;
        _isDirty = true;
    }
}

public record BuildingUpdated(
    HexCubeCoord Position,
    BuildingDefinition Definition,
    string EntityId,
    HexagonDirection Rotation,
    DesignationDefinition? Designation
    ) : IEntityMessage;

public record BuildingRemoved(
    HexCubeCoord Position,
    BuildingDefinition Definition,
    string EntityId,
    HexagonDirection Rotation
) : IEntityMessage;