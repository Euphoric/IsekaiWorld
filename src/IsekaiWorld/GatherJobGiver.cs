using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class GatherJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public GatherJobGiver(GameEntity game)
    {
        _game = game;
    }
    
    public IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character)
    {
        var toGather= 
            _game.Buildings
                .Where(x => x.Designation == DesignationDefinitions.Gather)
                .Where(x=> !x.ReservedForActivity)
                .FirstOrDefault()
            ;
        if (toGather == null)
            return null;

        toGather.ReservedForActivity = true;
        
        return new Activity[]
        {
            new MovementActivity(_game, _game.Pathfinding, character, toGather.Position.Neighbors()),
            new GatherActivity(_game, character, toGather)
        };
    }
}