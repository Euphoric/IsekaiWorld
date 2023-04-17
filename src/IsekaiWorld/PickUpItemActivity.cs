using System;

namespace IsekaiWorld;

public class PickUpItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private bool _executing;
    
    public PickUpItemActivity(GameEntity game, CharacterEntity character, ItemEntity item) : base(game)
    {
        _character = character;
        _item = item;
    }

    protected override void UpdateInner()
    {
        if (_character.Position != _item.Position)
        {
            throw new Exception("Character must stand on top of the item.");
        }

        if (!_executing)
        {
            _executing = true;
            
            var pickedItem = _item.PickUpItem(1);
            pickedItem.SetHolder(_character);
        }
        else
        {
            IsFinished = true;            
        }
    }
}