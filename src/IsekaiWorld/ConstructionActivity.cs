using System;

public class ConstructionActivity : IActivity
{
    private readonly GameEntity _game;
    public CharacterEntity Character { get; }
    public ConstructionEntity Construction { get; }

    private MovementActivity _movement;


    public bool IsFinished { get; private set; }

    public ConstructionActivity(GameEntity game, CharacterEntity character, ConstructionEntity construction)
    {
        _game = game;
        Character = character;
        Construction = construction;
    }

    public void Update()
    {
        if (IsFinished)
            return;

        bool isNextToConstruction =
            Construction.Position == Character.Position ||
            Character.Position.IsNextTo(Construction.Position);

        if (!isNextToConstruction)
        {
            if (_movement == null)
            {
                _movement = new MovementActivity(_game.Pathfinding, Character, Construction.Position, true);
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
                ConstructionEntity construction = Construction;
                if (construction.Definition.PlaceBuildingId != null)
                {
                    var buildingDefinition = BuildingDefinitions.GetById(construction.Definition.PlaceBuildingId);
                    _game.SpawnBuilding(construction.Position, construction.Rotation, buildingDefinition);
                }
                else if (construction.Definition.PlaceFloorId != null)
                {
                    // TODO
                }
                else
                {
                    throw new Exception("Invalid construction setup for: " + construction.Definition.Id);
                }
            }
        }
    }
}