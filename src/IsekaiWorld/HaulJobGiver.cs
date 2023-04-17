using System.Collections.Generic;
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

    public IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character)
    {
        var stockpilePositions = _game.Buildings.Where(x => x.Definition == BuildingDefinitions.StockpileZone)
            .SelectMany(x => x.OccupiedCells).Distinct().ToImmutableHashSet();

        var itemToHaul = _game.Items.FirstOrDefault(it => !stockpilePositions.Contains(it.Position));
        if (itemToHaul == null) 
            return null;
        
        var targetStockpile = _game.Buildings.FirstOrDefault(x => x.Definition == BuildingDefinitions.StockpileZone && (x.ReservedForItem == null || x.ReservedForItem == itemToHaul.Definition));
        if (targetStockpile == null) 
            return null;

        return new Activity[]
        {
            new MovementActivity(_game, _game.Pathfinding, character, itemToHaul.Position),
            new PickUpItemActivity(_game, character, itemToHaul),
            new MovementActivity(_game, _game.Pathfinding, character, targetStockpile.Position),
            new DropItemActivity(_game, character, itemToHaul, targetStockpile)
        };
    }
}