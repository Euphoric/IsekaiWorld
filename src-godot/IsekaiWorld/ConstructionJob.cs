public class ConstructionJob
{
    private readonly GameEntity _game;
    private readonly ConstructionEntity _construction;

    public bool InProgress { get; private set; }
    
    public ConstructionJob(GameEntity game, ConstructionEntity construction)
    {
        _game = game;
        _construction = construction;
    }

    public void StartWorking(CharacterEntity character)
    {
        InProgress = true;
        character.StartActivity(new ConstructionActivity(_game, character, _construction));
    }
}