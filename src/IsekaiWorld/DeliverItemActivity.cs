using System;

public class DeliverItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private readonly ConstructionEntity _construction;

    private ItemEntity? _carriedItem;
    private MovementActivity? _movement;
    private PickUpItemActivity? _pickUpItemActivity;

    public DeliverItemActivity(GameEntity game, CharacterEntity character, ItemEntity item,
        ConstructionEntity construction)
        : base(game)
    {
        _character = character;
        _item = item;
        _construction = construction;
    }

    protected override void UpdateInner()
    {
        if (_movement != null)
        {
            _movement.Update();
        }

        if (_pickUpItemActivity != null)
        {
            _pickUpItemActivity.Update();

            if (_pickUpItemActivity.IsFinished)
            {
                _carriedItem = _pickUpItemActivity.PickedUpItem;
                _pickUpItemActivity = null;
            }
        }
        else if (_carriedItem == null)
        {
            _pickUpItemActivity = new PickUpItemActivity(Game, _character, _item);
        }
        else
        {
            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(Game, Game.Pathfinding, _character, _construction.Position, false);
            }

            if (_movement.IsFinished)
            {
                _movement = null;

                _carriedItem.Remove();
                _carriedItem = null;

                _construction.MaterialsDelivered = true;

                IsFinished = true;
            }
        }
    }
}