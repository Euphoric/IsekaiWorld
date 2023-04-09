using System.Linq;

namespace IsekaiWorld;

public class EatFoodJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public EatFoodJobGiver(GameEntity game)
    {
        _game = game;
    }
    
    public bool SetJobActivity(CharacterEntity character)
    {
        if (character.Hunger < 0.3)
        {
            var foodItem = _game.Items.FirstOrDefault(x => x.Definition == ItemDefinitions.Grains);
            if (foodItem != null)
            {
                character.StartActivity(new EatActivity(_game, character, foodItem));
                return true;
            }
        }
        
        return false;
    }
}