using System;
using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class CharacterEntity : IEntity, IItemHolder
{
    public Guid Id { get; }
    public MessagingEndpoint Messaging { get; }

    public bool IsRemoved => false;
    
    private readonly GameEntity _game;

    public HexCubeCoord Position { get; set; }
    public ISet<HexCubeCoord> OccupiedCells => new HashSet<HexCubeCoord> { Position };

    private List<Activity> _activityList = new();

    public string Label { get; }

    private bool _initialized;

    public double Hunger { get; set; }
    public HexagonDirection FacingDirection { get; set; }
    public bool DisableHunger { get; set; }

    public IReadOnlyList<ItemEntity> CarriedItems => _carriedItems;

    public CharacterEntity(GameEntity game, string label)
    {
        Messaging = new(MessageHandler);
        
        Id = Guid.NewGuid();
        
        Label = label;
        _game = game;
        _initialized = false;
        Hunger = 1;
    }

    public void Initialize()
    {
    }

    public void Update()
    {
        if (!_initialized)
        {
            Messaging.Broadcast(new CharacterCreated(Id.ToString(), Label));
            _initialized = true;
        }
        
        if (!_activityList.Any())
        {
            _activityList = _game.Jobs.GetJobActivity(this)?.ToList() ?? new List<Activity>();
            foreach (var activity in _activityList)
            {
                activity.Reserve();
            }
        }

        Activity currentActivity = _activityList.FirstOrDefault() ?? new IdleActivity(_game);
        
        currentActivity.Update();

        if (currentActivity.IsFinished)
        {
            _activityList.Remove(currentActivity);
            currentActivity = _activityList.FirstOrDefault() ?? new IdleActivity(_game);
        }
        
        if (!_game.Paused)
        {
            if (!DisableHunger)
            {
                const double hungerRate = 0.0001;
                Hunger -= hungerRate;
            }
        }

        Messaging.Broadcast(new CharacterUpdated(Id.ToString(), Label, Position, FacingDirection, currentActivity.GetType().Name, Hunger));
    }

    public void Remove()
    {
        throw new NotImplementedException();
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
    
    private readonly List<ItemEntity> _carriedItems = new();

    void IItemHolder.RemoveItem(ItemEntity itemEntity)
    {
        _carriedItems.Remove(itemEntity);
    }

    void IItemHolder.AssignItem(ItemEntity itemEntity)
    {
        _carriedItems.Add(itemEntity);
    }
}

public record CharacterUpdated(String EntityId, String Label, HexCubeCoord Position, HexagonDirection FacingDirection, String ActivityName, double Hunger) : IEntityMessage;

public record SetCharacterHunger(String EntityId, double Hunger) : IEntityMessage;