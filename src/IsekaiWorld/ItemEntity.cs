using System;
using System.Collections.Generic;

namespace IsekaiWorld;

public interface IItemHolder
{
    void RemoveItem(ItemEntity itemEntity);
    void AssignItem(ItemEntity itemEntity);
}

public class ItemEntity : IEntity
{
    private readonly GameEntity _game;
    public MessagingEndpoint Messaging { get; } = new();

    private bool _toRemove;
    public bool IsRemoved { get; private set; }

    private bool _isDirty = true;

    private IItemHolder? _previousHolder;
    private IItemHolder? _holder;
    private HexCubeCoord _position;

    public ItemEntity(GameEntity game, HexCubeCoord position, ItemDefinition definition, int count)
    {
        EntityId = Guid.NewGuid();
        _game = game;
        _position = position;
        Definition = definition;
        Count = count;
    }

    public void SetHolder(IItemHolder holder)
    {
        _previousHolder = _holder;
        if (_holder != null)
        {
            _holder.RemoveItem(this);
            _holder = null;
        }
        _holder = holder;
        _holder?.AssignItem(this);

        _isDirty = true;
    }

    public Guid EntityId { get; }

    public HexCubeCoord Position
    {
        get => _position;
        set
        {
            _position = value;
            _isDirty = true;
        }
    }

    public ISet<HexCubeCoord> OccupiedCells => new HashSet<HexCubeCoord> { Position };
    public ItemDefinition Definition { get; }
    public int Count { get; private set; }
    
    public bool ReservedForActivity { get; set; }

    public void AddCount(int count)
    {
        Count += count;
        _isDirty = true;
    }

    public void Initialize()
    {
        Messaging.Broadcast(new ItemUpdated(EntityId.ToString(), Definition, Count, Position));
    }
    
    public void Update()
    {
        if (IsRemoved)
        {
            return;
        }

        if (_toRemove)
        {
            if (_holder is CharacterEntity ce)
            {
                Messaging.Broadcast(new ItemDropped(EntityId.ToString(), ce.Id.ToString()));
            }
            Messaging.Broadcast(new ItemRemoved(EntityId.ToString()));
            IsRemoved = true;
        }

        if (_isDirty)
        {
            _isDirty = false;
            // TODO: Refactor this holder mess
            if (_holder is MapItems)
            {
                if (_previousHolder is CharacterEntity ce)
                {
                    Messaging.Broadcast(new ItemDropped(EntityId.ToString(), ce.Id.ToString()));
                    Messaging.Broadcast(new ItemUpdated(EntityId.ToString(), Definition, Count, Position));
                    _previousHolder = null;
                }
                else
                {
                    Messaging.Broadcast(new ItemUpdated(EntityId.ToString(), Definition, Count, Position));
                }
            }
            else if (_holder is CharacterEntity ce)
            {
                Messaging.Broadcast(new ItemPickedUp(EntityId.ToString(), ce.Id.ToString()));
            }
        }
    }

    public void Remove()
    {
        _toRemove = true;
    }

    public ItemEntity PickUpItem(int pickUpCount)
    {
        ItemEntity pickedItem;
        if (Count > pickUpCount)
        {
            AddCount(-pickUpCount);
            var splitStack = new ItemEntity(_game, Position, Definition, pickUpCount);
            _game.AddEntity(splitStack);
            pickedItem = splitStack;
        }
        else
        {
            pickedItem = this;
        }

        return pickedItem;
    }
}

public record ItemUpdated(string EntityId, ItemDefinition Definition, int Count, HexCubeCoord Position) : IEntityMessage;

public record ItemRemoved(string EntityId) : IEntityMessage;

public record ItemPickedUp(string EntityId, string CharacterId) : IEntityMessage;

public record ItemDropped(string EntityId, string CharacterId) : IEntityMessage;