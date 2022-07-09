using System;
using System.Collections.Generic;

public interface IItemHolder
{
    void RemoveItem(ItemEntity itemEntity);
    void AssignItem(ItemEntity itemEntity);
}

public class ItemEntity : IEntity
{
    public EntityMessaging Messaging { get; } = new EntityMessaging();
    
    public bool IsRemoved => false;
    
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
        _holder.AssignItem(this);

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
        if (_isDirty)
        {
            _isDirty = false;
            if (_holder is MapItems)
            {
                yield return new UpdateItemOperation(this);
            }
            else
            {
                yield return new RemoveItemOperation(this);
            }
        }
    }
}