using System;
using System.Collections.Generic;

public class CharacterEntity : IEntity, IItemHolder
{
    public Guid Id { get; }
    public MessagingEndpoint Messaging { get; }

    public bool IsRemoved => false;
    
    private readonly GameEntity _game;

    public HexCubeCoord Position { get; set; }
    public ISet<HexCubeCoord> OccupiedCells => new HashSet<HexCubeCoord> { Position };
    
    public bool IsIdle => CurrentActivity == null;

    public Activity? CurrentActivity { get; private set; }
    public string Label { get; }

    private bool _initialized;

    public double Hunger { get; set; }
    
    public CharacterEntity(GameEntity game, string label)
    {
        Messaging = new(MessageHandler);
        
        Id = Guid.NewGuid();
        
        Label = label;
        _game = game;
        _initialized = false;
        Hunger = 1;
    }

    public void Update()
    {
        if (!_initialized)
        {
            Messaging.Broadcast(new CharacterCreated(Id.ToString(), Label));
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

        if (!_game.Paused)
        {
            const double hungerRate = 0.0001;
            Hunger -= hungerRate;
        }

        Messaging.Broadcast(new CharacterUpdated(Id.ToString(), Position, CurrentActivity?.GetType().Name, Hunger));
    }
    
    private void MessageHandler(IEntityMessage msgg)
    {
        if (msgg is SetCharacterHunger msg)
        {
            if (Id.ToString() == msg.EntityId)
            {
                Hunger = msg.Hunger;
            }
        }
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

public record CharacterUpdated(String EntityId, HexCubeCoord Position, String? ActivityName, double Hunger) : IEntityMessage;

public record SetCharacterHunger(String EntityId, double Hunger) : IEntityMessage;