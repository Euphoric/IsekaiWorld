using System.Linq;

namespace IsekaiWorld;

public class GatherJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public GatherJobGiver(GameEntity game)
    {
        _game = game;
    }
    
    public bool SetJobActivity(CharacterEntity character)
    {
        var toGather= _game.Buildings.FirstOrDefault(x => x.Designation == DesignationDefinitions.Gather);
        if (toGather == null)
            return false;

        character.StartActivity(new GatherActivity(_game, character, toGather));
        return true;
    }
}