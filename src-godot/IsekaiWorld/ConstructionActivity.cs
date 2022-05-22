public class ConstructionActivity : IActivity
{
    private readonly GameEntity _game;
    public CharacterEntity Character { get; }
    public ConstructionEntity Construction { get; }

    public bool IsFinished { get; private set; }
    
    public ConstructionActivity(GameEntity game, CharacterEntity character, ConstructionEntity construction)
    {
        _game = game;
        Character = character;
        Construction = construction;
    }

    public void Update(float delta)
    {
        if (IsFinished)
            return;
        
        Construction.Progress += delta;

        if (Construction.Progress > 3)
        {
            IsFinished = true;
            _game.RemoveConstruction(Construction);
        }
    }
}