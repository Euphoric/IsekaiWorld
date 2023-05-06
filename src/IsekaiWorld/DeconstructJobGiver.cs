using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class DeconstructJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public DeconstructJobGiver(GameEntity game)
    {
        _game = game;
    }

    public IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character)
    {
        var building = _game.Buildings
            .Where(x => x.Designation == DesignationDefinitions.Deconstruct)
            .Where(x=>!x.ReservedForActivity)
            .FirstOrDefault();
        
        if (building == null)
            return null;

        return new Activity[]
        {
            new MovementActivity(_game, _game.Pathfinding, character, building.Position.Neighbors()),
            new DeconstructActivity(_game, character, building)
        };
    }
}