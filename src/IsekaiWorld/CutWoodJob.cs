public class CutWoodJob : IJob
{
    public BuildingEntity Tree { get; }
    private readonly GameEntity _game;

    public CutWoodJob(GameEntity game, BuildingEntity tree)
    {
        Tree = tree;
        _game = game;
    }
    
    public bool InProgress { get; private set; } 
    
    public void StartWorking(CharacterEntity character)
    {
        InProgress = true;

        character.StartActivity(new CutTreeActivity(_game, character, Tree));
    }
}