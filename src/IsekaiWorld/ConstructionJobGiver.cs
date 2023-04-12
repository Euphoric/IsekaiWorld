using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class ConstructionJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public ConstructionJobGiver(GameEntity game)
    {
        _game = game;
    }

    public IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character)
    {
        var construction = _game.Constructions.FirstOrDefault();
        if (construction == null)
            return null;

        if (construction.Definition.Material != null && !construction.MaterialsDelivered)
        {
            var itemToDeliver = _game.Items.FirstOrDefault(x => x.Definition == construction.Definition.Material);
            if (itemToDeliver == null)
                return null;

            return new Activity[]
            {
                new PickUpItemActivity(_game, character, itemToDeliver),
                new MovementActivity(_game, _game.Pathfinding, character, construction.Position, false),
                new DeliverItemActivity(_game, character, itemToDeliver, construction)
            };
        }
        else
        {
            return new Activity[]
            {
                new MovementActivity(_game, _game.Pathfinding, character, construction.Position, true),
                new ConstructionActivity(_game, character, construction)
            };
        }
    }
}