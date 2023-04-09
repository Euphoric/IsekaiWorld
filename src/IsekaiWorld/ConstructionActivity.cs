using System;

namespace IsekaiWorld;

public class ConstructionActivity : Activity
{
    public CharacterEntity Character { get; }
    public ConstructionEntity Construction { get; }

    private MovementActivity? _movement;

    public ConstructionActivity(GameEntity game, CharacterEntity character, ConstructionEntity construction)
        :base(game)
    {
        Character = character;
        Construction = construction;
    }

    protected override void UpdateInner()
    {
        bool isNextToConstruction =
            Construction.Position == Character.Position ||
            Character.Position.IsNextTo(Construction.Position);

        if (!isNextToConstruction)
        {
            if (_movement == null)
            {
                _movement = new MovementActivity(Game, Game.Pathfinding, Character, Construction.Position, true);
            }

            _movement.Update();
        }
        else
        {
            _movement = null;

            Construction.AddProgress(1);

            if (Construction.IsFinished)
            {
                IsFinished = true;
                Construction.RemoveEntity();
                if (Construction.Definition.PlaceBuildingId != null)
                {
                    var buildingDefinition = BuildingDefinitions.GetById(Construction.Definition.PlaceBuildingId);
                    Game.SpawnBuilding(Construction.Position, Construction.Rotation, buildingDefinition);
                }
                else if (Construction.Definition.PlaceFloorId != null)
                {
                    // TODO
                }
                else
                {
                    throw new Exception("Invalid construction setup for: " + Construction.Definition.Id);
                }
            }
        }
    }
}