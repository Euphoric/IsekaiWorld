public class DeconstructActivity : IActivity
{
    private readonly GameEntity _game;
    public CharacterEntity Character { get; }
    public BuildingEntity Building { get; }

    private MovementActivity? _movement;
    
    public bool IsFinished { get; private set; }

    public DeconstructActivity(GameEntity game, CharacterEntity character, BuildingEntity building)
    {
        _game = game;
        Character = character;
        Building = building;
    }

    public void Update()
    {
        if (IsFinished)
            return;

        _movement?.Update();

        var canWork = Character.Position.IsNextTo(Building.Position);
        if (canWork)
        {
            _movement = null;

            Building.RemoveEntity();

            IsFinished = true;
        }
        else
        {
            // move on item
            _movement ??= new MovementActivity(_game.Pathfinding, Character, Building.Position, true);
        }
    }
}