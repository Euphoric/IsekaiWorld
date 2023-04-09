using System.Collections.Immutable;
using System.Linq;

namespace IsekaiWorld;

public class HaulJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public HaulJobGiver(GameEntity game)
    {
        _game = game;
    }

    public bool SetJobActivity(CharacterEntity character)
    {
        var stockpilePositions = _game.Buildings.Where(x => x.Definition == BuildingDefinitions.StockpileZone)
            .SelectMany(x => x.OccupiedCells).Distinct().ToImmutableHashSet();

        var itemToHaul = _game.Items.FirstOrDefault(it => !stockpilePositions.Contains(it.Position));
        if (itemToHaul == null) 
            return false;
        
        var targetStockpile = _game.Buildings.FirstOrDefault(x => x.Definition == BuildingDefinitions.StockpileZone && (x.ReservedForItem == null || x.ReservedForItem == itemToHaul.Definition));
        if (targetStockpile == null) 
            return false;
        
        character.StartActivity(new HaulItemActivity(_game, character, itemToHaul, targetStockpile));
        return true;
    }
}