using System.Linq;

namespace IsekaiWorld;

public class DeconstructActivityPlanner : IActivityPlanner
{
    private readonly GameEntity _game;

    public DeconstructActivityPlanner(GameEntity game)
    {
        _game = game;
    }

    public ActivityPlan? BuildPlan(CharacterEntity character)
    {
        var building = _game.Buildings
            .Where(x => x.Designation == DesignationDefinitions.Deconstruct)
            .Where(x => !x.ReservedForActivity)
            .FirstOrDefault();

        if (building == null)
            return null;

        return new ActivityPlan(
            new Activity[]
            {
                new MovementActivity(_game, _game.Pathfinding, character, building.Position.Neighbors()),
                new DeconstructActivity(_game, character, building)
            }
        );
    }
}