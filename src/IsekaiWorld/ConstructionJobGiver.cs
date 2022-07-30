using System.Linq;

public class ConstructionJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public ConstructionJobGiver(GameEntity game)
    {
        _game = game;
    }
    
    public bool SetJobActivity(CharacterEntity character)
    {
        var construction = _game.Constructions.FirstOrDefault();
        if (construction == null)
            return false;
        
        character.StartActivity(new ConstructionActivity(_game, character, construction));
        return true;
    }
}