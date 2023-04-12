using System.Linq;

namespace IsekaiWorld;

public class DeconstructJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public DeconstructJobGiver(GameEntity game)
    {
        _game = game;
    }

    public Activity? GetJobActivity(CharacterEntity character)
    {
        var buildingsToDeconstruct = 
            _game.Buildings.Where(x => x.Designation == DesignationDefinitions.Deconstruct);
        
        var building = buildingsToDeconstruct.FirstOrDefault();
        if (building == null)
            return null;
        
        return new DeconstructActivity(_game, character, building);
    }
}