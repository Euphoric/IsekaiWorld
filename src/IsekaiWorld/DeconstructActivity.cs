public class DeconstructActivity : Activity
{
    public CharacterEntity Character { get; }
    public BuildingEntity Building { get; }

    private MovementActivity? _movement;

    public DeconstructActivity(GameEntity game, CharacterEntity character, BuildingEntity building)
        :base(game)
    {
        Character = character;
        Building = building;
    }

    protected override void UpdateInner()
    {
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
            _movement ??= new MovementActivity(Game, Game.Pathfinding, Character, Building.Position, true);
        }
    }
}