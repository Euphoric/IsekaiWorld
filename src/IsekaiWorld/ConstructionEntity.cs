using System;
using System.Collections.Generic;
using System.Linq;

public class ConstructionEntity : IEntity
{
    public EntityMessaging Messaging { get; } = new EntityMessaging();
    
    private bool _isDirty;
    public bool IsRemoved { get; private set; }

    public Guid Id { get; }
 
    private float _progress;
    
    public HexCubeCoord Position { get; }
    public HexagonDirection Rotation { get; }
    public ISet<HexCubeCoord> OccupiedCells => new HashSet<HexCubeCoord> { Position };
    public ConstructionDefinition Definition { get; }

    public ConstructionEntity(HexCubeCoord position, HexagonDirection rotation, ConstructionDefinition definition)
    {
        Id = Guid.NewGuid();
        Position = position;
        Rotation = rotation;
        Definition = definition;
        
        _isDirty = true;
    }

    public float Progress
    {
        get => _progress;
        private set
        {
            _progress = value;
            _isDirty = true;
        }
    }

    public float CompleteProgress => Definition.WorkRequired;
    public float ProgressRelative => Progress / CompleteProgress;
    public bool IsFinished => Progress >= CompleteProgress;
    public bool MaterialsDelivered { get; set; }

    public IEnumerable<INodeOperation> Update()
    {
        if (IsRemoved)
        {
            Messaging.Broadcast(new ConstructionRemoved(Id.ToString()));
            return Enumerable.Empty<INodeOperation>();
        }
        
        if (_isDirty)
        {
            Messaging.Broadcast(new ConstructionUpdated(Id.ToString(), Position, ProgressRelative));
            _isDirty = false;
        }
        
        return Enumerable.Empty<INodeOperation>();
    }

    public void RemoveEntity()
    {
        IsRemoved = true;
        _isDirty = true;
    }

    public void AddProgress(float progress)
    {
        Progress += progress;
    }
}

public class ConstructionUpdated : IEntityMessage
{
    public ConstructionUpdated(string id, HexCubeCoord position, float progressRelative)
    {
        Id = id;
        Position = position;
        ProgressRelative = progressRelative;
    }

    public String Id { get; }
    public HexCubeCoord Position { get; }
    public float ProgressRelative { get; }
}

public class ConstructionRemoved : IEntityMessage
{
    public ConstructionRemoved(string id)
    {
        Id = id;
    }

    public String Id { get; }
}