using System;
using System.Linq;

public class HaulItemActivity : IActivity
{
    private readonly GameEntity _game;
    public CharacterEntity Character { get; }
    public ItemEntity Item { get; }
    public HexCubeCoord DropOffPosition { get; }

    private MovementActivity _movement;

    private bool _isPickedUp;
    public bool IsFinished { get; private set; }

    public HaulItemActivity(GameEntity game, CharacterEntity character, ItemEntity item, HexCubeCoord dropOffPosition)
    {
        _game = game;
        Character = character;
        Item = item;
        DropOffPosition = dropOffPosition;
    }

    public void Update(float delta)
    {
        if (IsFinished)
            return;

        if (_movement != null)
        {
            _movement.Update(delta);
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
                _movement = new MovementActivity(_game.Pathfinding, Character, DropOffPosition, false);
            }

            if (_movement.IsFinished)
            {
                _movement = null;
                _isPickedUp = false;

                var itemInPlace = _game.Items.FirstOrDefault(x => x.Position == DropOffPosition && x.Definition == Item.Definition);
                if (itemInPlace == null)
                {
                    // place item on ground
                    Item.Position = DropOffPosition;
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