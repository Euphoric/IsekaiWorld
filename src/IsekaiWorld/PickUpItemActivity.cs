public class PickUpItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private MovementActivity? _movement;

    public ItemEntity? PickedUpItem { get; private set; }

    public PickUpItemActivity(GameEntity game, CharacterEntity character, ItemEntity item) : base(game)
    {
        _character = character;
        _item = item;
    }

    protected override void UpdateInner()
    {
        if (_movement == null)
        {
            // move on item
            _movement = new MovementActivity(Game, Game.Pathfinding, _character, _item.Position, false);
        }

        if (_movement != null)
        {
            _movement.Update();
            
            if (!_movement.IsFinished)
                return;
        }

        _movement = null;

        var pickedItem = _item.PickUpItem(1);

        pickedItem.SetHolder(_character);
        PickedUpItem = pickedItem;
        IsFinished = true;
    }
}