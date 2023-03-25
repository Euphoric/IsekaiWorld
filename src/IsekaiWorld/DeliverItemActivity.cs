using System;

public class DeliverItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private readonly ConstructionEntity _construction;

    private ItemEntity? _carriedItem;
    private MovementActivity? _moveToConstructionActivity;
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
        if (_carriedItem == null)
        {
            if (_pickUpItemActivity == null)
            {
                _pickUpItemActivity = new PickUpItemActivity(Game, _character, _item);
            }
            else
            {
                _pickUpItemActivity.Update();

                if (_pickUpItemActivity.IsFinished)
                {
                    _carriedItem = _pickUpItemActivity.PickedUpItem;
                    _pickUpItemActivity = null;
                }
            }
        }
        else
        {
            if (_moveToConstructionActivity == null)
            {
                _moveToConstructionActivity = new MovementActivity(Game, Game.Pathfinding, _character, _construction.Position, false);
            }
            else
            {
                _moveToConstructionActivity.Update();

                if (_moveToConstructionActivity.IsFinished)
                {
                    _moveToConstructionActivity = null;

                    _carriedItem.Remove();
                    _carriedItem = null;

                    _construction.MaterialsDelivered = true;

                    IsFinished = true;
                }
            }
        }
    }
}