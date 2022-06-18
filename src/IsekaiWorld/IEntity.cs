using System.Collections.Generic;

public interface IEntity
{
    bool IsRemoved { get; }

    ISet<HexCubeCoord> OccupiedCells { get; }

    IEnumerable<INodeOperation> Update();
}