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
            Messaging.Broadcast(new ItemPickedUp(EntityId.ToString()));
            IsRemoved = true;
        }

        if (_isDirty)
        {
            _isDirty = false;
            if (_holder is MapItems)
            {
                Messaging.Broadcast(new ItemUpdated(EntityId.ToString(), Definition, Count, Position));
            }
            else
            {
                Messaging.Broadcast(new ItemPickedUp(EntityId.ToString()));
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

public class ItemUpdated : IEntityMessage
{
    public ItemUpdated(string entityId, ItemDefinition definition, int count, HexCubeCoord position)
    {
        EntityId = entityId;
        Definition = definition;
        Count = count;
        Position = position;
    }

    public String EntityId { get; }
    public ItemDefinition Definition { get; }
    public int Count { get; }
    public HexCubeCoord Position { get; }
}

public class ItemPickedUp : IEntityMessage
{
    public ItemPickedUp(string entityId)
    {
        EntityId = entityId;
    }

    public String EntityId { get; }
}