using System;
using System.Collections.Generic;

public class ItemEntity
{
    private bool _isDirty = true;
    
    public ItemEntity(HexCubeCoord position, ItemDefinition definition, int count)
    {
        EntityId = Guid.NewGuid();
        Position = position;
        Definition = definition;
        Count = count;
    }

    public Guid EntityId { get; }
    public HexCubeCoord Position { get; }
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
            yield return new UpdateItemOperation(this);
        }
    }
}