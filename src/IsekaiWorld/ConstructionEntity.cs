using System;
using System.Collections.Generic;

public class ConstructionEntity : IEntity
{
    private bool _isDirty;
    public bool IsRemoved { get; private set; }

    public Guid Id { get; }
 
    private float _progress;
    
    public HexCubeCoord Position { get; }
    public BuildingDefinition BuildingDefinition { get; }

    public ConstructionEntity(HexCubeCoord position, BuildingDefinition buildingDefinition)
    {
        Id = Guid.NewGuid();
        Position = position;
        BuildingDefinition = buildingDefinition;
        
        _isDirty = true;
    }

    public float Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            _isDirty = true;
        }
    }

    public float CompleteProgress => 3;
    public float ProgressRelative => Progress / CompleteProgress;

    public IEnumerable<INodeOperation> Update()
    {
        if (IsRemoved)
        {
            yield return new RemoveConstruction(this);
            yield break;
        }
        
        if (_isDirty)
        {
            yield return new UpdateConstruction(this);
            _isDirty = false;
        }
    }

    public void RemoveEntity()
    {
        IsRemoved = true;
        _isDirty = true;
    }
}