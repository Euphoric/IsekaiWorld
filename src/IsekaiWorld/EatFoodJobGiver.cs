using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class EatFoodJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public EatFoodJobGiver(GameEntity game)
    {
        _game = game;
    }

    public IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character)
    {
        if (character.Hunger < 0.3)
        {
            var foodItem = _game.Items.FirstOrDefault(x => x.Definition == ItemDefinitions.Grains);
            if (foodItem != null)
            {
                return new Activity[]
                {
                    new MovementActivity(_game, _game.Pathfinding, character, foodItem.Position),
                    new PickUpItemActivity(_game, character, foodItem),
                    new EatActivity(_game, character, foodItem)
                };
            }
        }

        return null;
    }
}