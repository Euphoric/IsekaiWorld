using System;

public class DeliverItemActivity : IActivity
{
    private readonly GameEntity _game;
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private readonly ConstructionEntity _construction;

    private bool _isPickedUp;
    private MovementActivity _movement;
    
    public DeliverItemActivity(GameEntity game, CharacterEntity character, ItemEntity item, ConstructionEntity construction)
    {
        _game = game;
        _character = character;
        _item = item;
        _construction = construction;
    }
    
    public bool IsFinished { get; private set; }

    public void Update()
    {
        if (IsFinished)
            return;
        
        if (_movement != null)
        {
            _movement.Update();
        }
        
        if (!_isPickedUp)
        {
            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(_game.Pathfinding, _character, _item.Position, false);
            }

            if (_movement.IsFinished)
            {
                _movement = null;

                _item.SetHolder(_character);
                
                _isPickedUp = true;
            }
        }
        else
        {
            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(_game.Pathfinding, _character, _construction.Position, false);
            }
            
            if (_movement.IsFinished)
            {
                _movement = null;
                _isPickedUp = false;

                _item.Remove();

                _construction.MaterialsDelivered = true;

                IsFinished = true;
            }
        }
    }
}