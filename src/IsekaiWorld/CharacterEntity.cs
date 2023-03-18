using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterEntity : IEntity, IItemHolder
{
    public Guid Id { get; }
    public MessagingEndpoint Messaging { get; } = new MessagingEndpoint();
    
    public bool IsRemoved => false;
    
    private readonly GameEntity _game;

    public HexCubeCoord Position { get; set; }
    public ISet<HexCubeCoord> OccupiedCells => new HashSet<HexCubeCoord> { Position };
    
    public bool IsIdle => CurrentActivity == null;

    public Activity? CurrentActivity { get; private set; }
    public string Label { get; private set; }

    private bool _initialized;
    
    public CharacterEntity(GameEntity game, string label)
    {
        Id = Guid.NewGuid();
        
        Label = label;
        _game = game;
        _initialized = false;
    }

    public void Update()
    {
        if (!_initialized)
        {
            Messaging.Broadcast(new CharacterCreated(Id.ToString()));
            _initialized = true;
        }
        
        if (CurrentActivity == null)
        {
            _game.Jobs.SetJobActivity(this);
        }
        
        CurrentActivity?.Update();
        
        if (CurrentActivity != null && CurrentActivity.IsFinished)
        {
            CurrentActivity = null;
        }

        Messaging.Broadcast(new CharacterUpdated(Id.ToString(), Position));
    }

    public void StartActivity(Activity activity)
    {
        CurrentActivity = activity;
    }

    private List<ItemEntity> _carriedItems = new();

    void IItemHolder.RemoveItem(ItemEntity itemEntity)
    {
        _carriedItems.Remove(itemEntity);
    }

    void IItemHolder.AssignItem(ItemEntity itemEntity)
    {
        _carriedItems.Add(itemEntity);
    }
}

public class CharacterUpdated : IEntityMessage
{
    public string EntityId { get; }
    public HexCubeCoord Position { get; }

    public CharacterUpdated(String entityId, HexCubeCoord position)
    {
        EntityId = entityId;
        Position = position;
    }
}