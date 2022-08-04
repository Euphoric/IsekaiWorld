using System;
using System.Collections.Generic;
using System.Linq;

public interface IItemHolder
{
    void RemoveItem(ItemEntity itemEntity);
    void AssignItem(ItemEntity itemEntity);
}

public class ItemEntity : IEntity
{
    public EntityMessaging Messaging { get; } = new EntityMessaging();

    private bool _toRemove;
    public bool IsRemoved { get; private set; }

    private bool _isDirty = true;

    private IItemHolder _holder;
    private HexCubeCoord _position;

    public ItemEntity(HexCubeCoord position, ItemDefinition definition, int count)
    {
        EntityId = Guid.NewGuid();
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

    public IEnumerable<INodeOperation> Update()
    {
        if (IsRemoved)
        {
            return Enumerable.Empty<INodeOperation>();
        }

        if (_toRemove)
        {
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

        return Enumerable.Empty<INodeOperation>();
    }

    public void Remove()
    {
        _toRemove = true;
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