using System.Collections.Generic;

public interface IEntity
{
    bool IsRemoved { get; }
    
    IEnumerable<INodeOperation> Update();
}