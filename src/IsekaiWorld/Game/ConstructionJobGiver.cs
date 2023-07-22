using System.Linq;

namespace IsekaiWorld.Game;

public class ConstructionActivityPlanner : IActivityPlanner
{
    private readonly GameEntity _game;

    public ConstructionActivityPlanner(GameEntity game)
    {
        _game = game;
    }

    public ActivityPlan? BuildPlan(CharacterEntity character)
    {
        var construction =
            _game.Constructions
                .Where(x => !x.ReservedForActivity)
                .FirstOrDefault();
        if (construction == null)
            return null;

        if (construction.Definition.Material != null && !construction.MaterialsDelivered)
        {
            var itemToDeliver =
                _game.MapItems
                    .Where(x => x.Definition == construction.Definition.Material)
                    .Where(x => !x.ReservedForActivity)
                    .FirstOrDefault();
            if (itemToDeliver == null)
                return null;

            return new ActivityPlan(
                new Activity[]
                {
                    new MovementActivity(_game, _game.Pathfinding, character, itemToDeliver.Position),
                    new PickUpItemActivity(_game, character, itemToDeliver, 1),
                    new MovementActivity(_game, _game.Pathfinding, character, construction.Position.Neighbors()),
                    new DeliverItemActivity(_game, character, itemToDeliver, construction)
                });
        }
        else
        {
            return new ActivityPlan(
                new Activity[]
                {
                    new MovementActivity(_game, _game.Pathfinding, character, construction.Position.Neighbors()),
                    new ConstructionActivity(_game, character, construction)
                });
        }
    }
}