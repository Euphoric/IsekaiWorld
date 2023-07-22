using System;
using System.Linq;

namespace IsekaiWorld.Game;

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
                .Where(x => !x.ReservedForActivity)
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

    public override void Reserve()
    {
        _craftingStation.ReservedForActivity = true;
    }

    private int _craftingTimer = 0;
    
    protected override void UpdateInner()
    {
        var interactionPoint = _craftingStation.Position + HexagonDirection.TopLeft;
        if (_character.Position != interactionPoint)
        {
            throw new Exception("Character must be on the interaction spot");
        }
        
        _craftingTimer++;

        if (_craftingTimer >= GameSpeed.BaseTps * 3)
        {
            Game.CraftingFinished();
            Game.SpawnItem(_craftingStation.Position, _billToCraft.Item, 1);
            IsFinished = true;
            _craftingStation.ReservedForActivity = false;            
        }
    }
}