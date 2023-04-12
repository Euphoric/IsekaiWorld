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

            return new[] { new DeliverItemActivity(_game, character, itemToDeliver, construction) };
        }
        else
        {
            return new[] { new ConstructionActivity(_game, character, construction) };
        }
    }
}