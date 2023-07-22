using System;
using System.Collections.Generic;

namespace IsekaiWorld.Game;

public class ConstructionEntity : IEntity
{
    public MessagingEndpoint Messaging { get; } = new MessagingEndpoint();

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

    public float CompleteProgress => Definition.ConstructionTime;
    public float ProgressRelative => Progress / CompleteProgress;
    public bool IsFinished => Progress >= CompleteProgress;
    public bool MaterialsDelivered { get; set; }
    
    public bool ReservedForActivity { get; set; }

    public void Initialize()
    {
    }

    public void Update()
    {
        if (IsRemoved)
        {
            Messaging.Broadcast(new ConstructionRemoved(Id.ToString()));
        }
        else if (_isDirty)
        {
            Messaging.Broadcast(new ConstructionUpdated(Id.ToString(), Definition, Position, ProgressRelative));
            _isDirty = false;
        }
    }

    public void Remove()
    {
        IsRemoved = true;
        _isDirty = true;
    }

    public void AddProgress(float progress)
    {
        Progress += progress;
    }
}

public record ConstructionUpdated(string EntityId, ConstructionDefinition Definition, HexCubeCoord Position, float ProgressRelative) : IEntityMessage;

public record ConstructionRemoved(string EntityId) : IEntityMessage;