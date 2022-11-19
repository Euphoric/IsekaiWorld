using System.Collections.Generic;

public interface IEntity
{
    EntityMessaging Messaging { get; }
    
    bool IsRemoved { get; }

    ISet<HexCubeCoord> OccupiedCells { get; }

    void Update();
}