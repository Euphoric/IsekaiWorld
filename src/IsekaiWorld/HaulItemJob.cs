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
        var stockpile = _game.Buildings.First(x => x.Definition == BuildingDefinitions.StockpileZone);
        
        character.StartActivity(new HaulItemActivity(_game, character, Item, stockpile.Position));
    }
}