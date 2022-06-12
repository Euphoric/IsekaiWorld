using System.Collections.Generic;

public interface IEntity
{
    bool IsRemoved { get; }
    
    HexCubeCoord Position { get; }

    IEnumerable<INodeOperation> Update();
}