using System.Linq;

namespace IsekaiWorld;

public class DeliverItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private readonly ConstructionEntity _construction;
    
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
        var carriedItem = _character.CarriedItems.FirstOrDefault(x => x.Definition == _item.Definition && x.Count == 1);
        if (carriedItem == null)
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

                    (_character as IItemHolder).RemoveItem(carriedItem); // TODO: Remove carried item when item is removed?
                    carriedItem.Remove();

                    _construction.MaterialsDelivered = true;

                    IsFinished = true;
                }
            }
        }
    }
}