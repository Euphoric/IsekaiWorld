using System.Collections.Generic;

public interface IEntity
{
    MessagingEndpoint Messaging { get; }
    
    bool IsRemoved { get; }

    ISet<HexCubeCoord> OccupiedCells { get; }

    void Update();
}