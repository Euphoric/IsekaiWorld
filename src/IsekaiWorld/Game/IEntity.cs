using System.Collections.Generic;

namespace IsekaiWorld.Game;

public interface IEntity
{
    MessagingEndpoint Messaging { get; }
    
    bool IsRemoved { get; }

    ISet<HexCubeCoord> OccupiedCells { get; }

    void Initialize();
    void Update();
    void Remove();
}