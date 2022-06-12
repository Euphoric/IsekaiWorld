using System.Collections.Generic;

public interface IEntity
{
    IEnumerable<INodeOperation> Update();
}