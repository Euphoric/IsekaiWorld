using System;
using System.Collections.Generic;

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
        set
        {
            _progress = value;
            _isDirty = true;
        }
    }

    public float CompleteProgress => 3;
    public float ProgressRelative => Progress / CompleteProgress;

    public IEnumerable<INodeOperation> Update()
    {
        if (IsRemoved)
        {
            yield return new RemoveConstruction(this);
            yield break;
        }
        
        if (_isDirty)
        {
            yield return new UpdateConstruction(this);
            _isDirty = false;
        }
    }

    public void RemoveEntity()
    {
        IsRemoved = true;
        _isDirty = true;
    }
}