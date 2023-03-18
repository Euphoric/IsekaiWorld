using System.Linq;

public class HaulItemActivity : Activity
{
    public CharacterEntity Character { get; }
    public ItemEntity Item { get; }

    private MovementActivity? _movement;

    private bool _isPickedUp;
    
    private readonly BuildingEntity _targetStockpile;

    public HaulItemActivity(GameEntity game, CharacterEntity character, ItemEntity item, BuildingEntity targetStockpile)
        :base(game)
    {
        _targetStockpile = targetStockpile;
        Character = character;
        Item = item;
    }

    protected override void UpdateInner()
    {
        _targetStockpile.ReserveForItem(Item.Definition);

        _movement?.Update();

        if (!_isPickedUp)
        {
            // move on item
            _movement ??= new MovementActivity(Game, Game.Pathfinding, Character, Item.Position, false);

            if (_movement.IsFinished)
            {
                _movement = null;

                Item.SetHolder(Character);
                
                _isPickedUp = true;
            }
        }
        else
        {
            // move on item
            _movement ??= new MovementActivity(Game, Game.Pathfinding, Character, _targetStockpile.Position, false);

            if (_movement.IsFinished)
            {
                _movement = null;
                _isPickedUp = false;

                var itemInPlace = Game.Items.FirstOrDefault(x => x.Position == _targetStockpile.Position && x.Definition == Item.Definition);
                if (itemInPlace == null)
                {
                    // place item on ground
                    Item.Position = _targetStockpile.Position;
                    Item.SetHolder(Game.MapItems);
                }
                else
                {
                    // stack items together
                    itemInPlace.AddCount(Item.Count);
                    Item.Remove();
                }

                IsFinished = true;
            }
        }
    }
}