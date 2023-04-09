public class GatherActivity : Activity
{
    public CharacterEntity Character { get; }
    public BuildingEntity EntityToGather { get; }

    private MovementActivity? _movement;

    public GatherActivity(GameEntity game, CharacterEntity character, BuildingEntity entityToGather)
        :base(game)
    {
        Character = character;
        EntityToGather = entityToGather;
    }

    protected override void UpdateInner()
    {
        bool isNextToEntity =
            EntityToGather.Position == Character.Position ||
            Character.Position.IsNextTo(EntityToGather.Position);

        if (!isNextToEntity)
        {
            if (_movement == null)
            {
                _movement = new MovementActivity(Game, Game.Pathfinding, Character, EntityToGather.Position, true);
            }

            _movement.Update();
        }
        else
        {
            _movement = null;
            
            EntityToGather.RemoveEntity();
            Game.SpawnItem(EntityToGather.Position, ItemDefinitions.Grains, 1);
            
            IsFinished = true;
        }
    }
}