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

    public IActivity CurrentActivity { get; private set; }
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
        
        if (CurrentActivity != null && CurrentActivity.IsFinished)
        {
            CurrentActivity = null;
        }
        
        if (CurrentActivity == null)
        {
            var job = _game.Jobs.GetNextJob(this);
            if (job != null)
            {
                job.StartWorking(this);
                _game.RunActivity(CurrentActivity);
            }
        }

        yield return new UpdateCharacter(this);
    }

    public void StartActivity(IActivity activity)
    {
        CurrentActivity = activity;
    }

    private List<ItemEntity> _carriedItems = new List<ItemEntity>();

    void IItemHolder.RemoveItem(ItemEntity itemEntity)
    {
        _carriedItems.Remove(itemEntity);
    }

    void IItemHolder.AssignItem(ItemEntity itemEntity)
    {
        _carriedItems.Add(itemEntity);
    }
}