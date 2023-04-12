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
        var toGather= _game.Buildings.FirstOrDefault(x => x.Designation == DesignationDefinitions.Gather);
        if (toGather == null)
            return null;

        return new []{ new GatherActivity(_game, character, toGather) };
    }
}