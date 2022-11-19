using System.Linq;

public class DeconstructJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public DeconstructJobGiver(GameEntity game)
    {
        _game = game;
    }

    public bool SetJobActivity(CharacterEntity character)
    {
        var buildingsToDeconstruct = 
            _game.Buildings.Where(x => x.Designation == DesignationDefinitions.Deconstruct);
        
        var building = buildingsToDeconstruct.FirstOrDefault();
        if (building == null)
            return false;
        
        character.StartActivity(new DeconstructActivity(_game, character, building));
        return true;
    }
}