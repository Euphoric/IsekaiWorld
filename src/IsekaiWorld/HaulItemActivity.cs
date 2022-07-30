using System;
using System.Linq;

public class HaulItemActivity : IActivity
{
    private readonly GameEntity _game;
    public CharacterEntity Character { get; }
    public ItemEntity Item { get; }

    private MovementActivity _movement;

    private bool _isPickedUp;
    
    private readonly BuildingEntity _targetStockpile;
    public bool IsFinished { get; private set; }

    public HaulItemActivity(GameEntity game, CharacterEntity character, ItemEntity item, BuildingEntity targetStockpile)
    {
        _game = game;
        _targetStockpile = targetStockpile;
        Character = character;
        Item = item;
    }

    public void Update()
    {
        if (IsFinished)
            return;
        
        _targetStockpile.ReserveForItem(Item.Definition);

        if (_movement != null)
        {
            _movement.Update();
        }

        if (!_isPickedUp)
        {
            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(_game.Pathfinding, Character, Item.Position, false);
            }

            if (_movement.IsFinished)
            {
                _movement = null;

                Item.SetHolder(Character);
                
                _isPickedUp = true;
            }
        }
        else
        {
            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(_game.Pathfinding, Character, _targetStockpile.Position, false);
            }

            if (_movement.IsFinished)
            {
                _movement = null;
                _isPickedUp = false;

                var itemInPlace = _game.Items.FirstOrDefault(x => x.Position == _targetStockpile.Position && x.Definition == Item.Definition);
                if (itemInPlace == null)
                {
                    // place item on ground
                    Item.Position = _targetStockpile.Position;
                    Item.SetHolder(_game.MapItems);
                }
                else
                {
                    // stack items together
                    itemInPlace.AddCount(Item.Count);
                    _game.RemoveEntity(Item);
                    Item.SetHolder(null);
                }

                IsFinished = true;
            }
        }
    }
}