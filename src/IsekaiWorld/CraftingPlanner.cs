using System;
using System.Linq;

namespace IsekaiWorld;

public class CraftingPlanner : IActivityPlanner
{
    private readonly GameEntity _game;

    public CraftingPlanner(GameEntity game)
    {
        _game = game;
    }

    public ActivityPlan? BuildPlan(CharacterEntity character)
    {
        var craftinStation =
            _game.Buildings
                .Where(x => x.Definition == BuildingDefinitions.CraftingDesk)
                .FirstOrDefault();

        if (_game.BillToCraft != null && craftinStation != null)
        {
            var interactionSpot = craftinStation.Position + HexagonDirection.TopLeft;
            
            return new ActivityPlan(
                new Activity[]
                {
                    new MovementActivity(_game, _game.Pathfinding, character, interactionSpot),
                    new CraftingActivity(_game, character, craftinStation, _game.BillToCraft)
                }
            );
        }
        else
        {
            return null;
        }
    }
}

public class CraftingActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly BuildingEntity _craftingStation;
    private readonly CraftingDefinition _billToCraft;

    public CraftingActivity(
        GameEntity game,
        CharacterEntity character,
        BuildingEntity craftingStation,
        CraftingDefinition billToCraft
    ) : base(game)
    {
        _character = character;
        _craftingStation = craftingStation;
        _billToCraft = billToCraft;
    }

    protected override void UpdateInner()
    {
        var interactionPoint = _craftingStation.Position + HexagonDirection.TopLeft;
        if (_character.Position != interactionPoint)
        {
            throw new Exception("Character must be on the interaction spot");
        }
        Game.SpawnItem(_craftingStation.Position, _billToCraft.Item, 1);
        IsFinished = true;
    }
}