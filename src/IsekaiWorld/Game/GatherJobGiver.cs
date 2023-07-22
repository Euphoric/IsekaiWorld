using System.Linq;

namespace IsekaiWorld.Game;

public class GatherActivityPlanner : IActivityPlanner
{
    private readonly GameEntity _game;

    public GatherActivityPlanner(GameEntity game)
    {
        _game = game;
    }

    public ActivityPlan? BuildPlan(CharacterEntity character)
    {
        var toGather =
                _game.Buildings
                    .Where(x => x.Designation == DesignationDefinitions.Gather)
                    .Where(x => !x.ReservedForActivity)
                    .FirstOrDefault()
            ;
        if (toGather == null)
            return null;

        return new ActivityPlan(
            new Activity[]
            {
                new MovementActivity(_game, _game.Pathfinding, character, toGather.Position.Neighbors()),
                new GatherActivity(_game, character, toGather)
            }
        );
    }
}