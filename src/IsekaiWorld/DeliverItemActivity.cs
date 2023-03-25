using System;

public class DeliverItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private readonly ConstructionEntity _construction;
    
    private ItemEntity? _carriedItem;
    private MovementActivity? _movement;
    
    public DeliverItemActivity(GameEntity game, CharacterEntity character, ItemEntity item, ConstructionEntity construction)
        :base(game)
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
        
        if (_carriedItem == null)
        {
            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(Game, Game.Pathfinding, _character, _item.Position, false);
            }

            var pickUpCount = 1;
            if (_movement.IsFinished)
            {
                _movement = null;

                var pickedItem = _item.PickUpItem(pickUpCount);
                
                pickedItem.SetHolder(_character);
                _carriedItem = pickedItem;
            }
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