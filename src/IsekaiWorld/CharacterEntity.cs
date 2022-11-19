using System;
using System.Collections.Generic;

public class CharacterEntity : IEntity, IItemHolder
{
    public Guid Id { get; }
    public EntityMessaging Messaging { get; } = new EntityMessaging();
    
    public bool IsRemoved => false;
    
    private readonly GameEntity _game;

    public HexCubeCoord Position { get; set; }
    public ISet<HexCubeCoord> OccupiedCells => new HashSet<HexCubeCoord> { Position };
    
    public bool IsIdle => CurrentActivity == null;

    public IActivity? CurrentActivity { get; private set; }
    public string Label { get; private set; }

    private bool _initialized;
    
    public CharacterEntity(GameEntity game, string label)
    {
        Id = Guid.NewGuid();
        
        Label = label;
        _game = game;
        _initialized = false;
    }

    public IEnumerable<INodeOperation> Update()
    {
        if (!_initialized)
        {
            yield return new CreateCharacter(this);
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
        yield return new UpdateCharacter(this);
    }

    public void StartActivity(IActivity activity)
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
    public string Id { get; }
    public HexCubeCoord Position { get; }

    public CharacterUpdated(String id, HexCubeCoord position)
    {
        Id = id;
        Position = position;
    }
}