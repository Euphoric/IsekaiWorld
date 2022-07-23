using System.Linq;

public class HaulItemJob : IJob
{
    public ItemEntity Item { get; }
    private readonly GameEntity _game;

    public HaulItemJob(GameEntity game, ItemEntity item)
    {
        Item = item;
        _game = game;
    }
    
    public bool InProgress { get; private set; } 
    
    public void StartWorking(CharacterEntity character)
    {
        InProgress = true;

        character.StartActivity(new HaulItemActivity(_game, character, Item));
    }
}