using System.Collections.Immutable;
using System.Linq;

namespace IsekaiWorld.Game;

public class HaulActivityPlanner : IActivityPlanner
{
    private readonly GameEntity _game;

    public HaulActivityPlanner(GameEntity game)
    {
        _game = game;
    }

    public ActivityPlan? BuildPlan(CharacterEntity character)
    {
        var stockpilePositions =
            _game.Buildings
                .Where(x => x.Definition == BuildingDefinitions.StockpileZone)
                .SelectMany(x => x.OccupiedCells)
                .Distinct().ToImmutableHashSet();

        var itemToHaul =
            _game.MapItems
                .Where(it => !stockpilePositions.Contains(it.Position))
                .Where(it => !it.ReservedForActivity)
                .FirstOrDefault();
        if (itemToHaul == null)
            return null;

        var itemsByPosition =
            _game.MapItems.GroupBy(x => x.Position)
                .ToImmutableDictionary(x => x.Key, x => x.Select(i => i.Definition).ToImmutableHashSet());

        var targetStockpile =
            _game.Buildings
                .Where(x => x.Definition == BuildingDefinitions.StockpileZone)
                .Where(s => itemsByPosition.GetValueOrDefault(s.Position)?.Contains(itemToHaul.Definition) ?? true)
                .Where(x => !x.ReservedForActivity)
                .FirstOrDefault();
        if (targetStockpile == null)
            return null;

        return new ActivityPlan(
            new Activity[]
            {
                new MovementActivity(_game, _game.Pathfinding, character, itemToHaul.Position),
                new PickUpItemActivity(_game, character, itemToHaul, null),
                new MovementActivity(_game, _game.Pathfinding, character, targetStockpile.Position),
                new DropItemActivity(_game, character, itemToHaul, targetStockpile)
            }
        );
    }
}