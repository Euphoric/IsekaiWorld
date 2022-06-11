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

    public void Update(float delta)
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
                _movement = new MovementActivity(_game.Pathfinding, Character, Construction.Position);
            }

            _movement.Update(delta);
        }
        else
        {
            _movement = null;

            Construction.Progress += delta;

            if (Construction.Progress > 3)
            {
                IsFinished = true;
                _game.RemoveConstruction(Construction);
                ConstructionEntity construction = Construction;
                _game.SpawnBuilding(construction.Position, construction.BuildingDefinition);
            }
        }
    }
}