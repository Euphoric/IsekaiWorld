using System.Collections.Generic;

public class BuildingEntity : IEntity
{
    private bool _isDirty;

    public BuildingEntity(HexCubeCoord position, BuildingDefinition definition)
    {
        Position = position;
        Definition = definition;
        _isDirty = true;
    }

    public HexCubeCoord Position { get; }
    public BuildingDefinition Definition { get; }
    public string Label => Definition.Label;

    public bool IsRemoved => false;

    public IEnumerable<INodeOperation> Update()
    {
        if (_isDirty)
        {
            yield return new HexagonalMapEntity.RefreshMapOperation();
            _isDirty = false;
        }
    }
}