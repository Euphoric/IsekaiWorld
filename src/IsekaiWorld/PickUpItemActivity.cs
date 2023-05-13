using System;

namespace IsekaiWorld;

public class PickUpItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private readonly int? _count;
    private bool _executing;
    
    public PickUpItemActivity(GameEntity game, CharacterEntity character, ItemEntity item, int? count) : base(game)
    {
        _character = character;
        _item = item;
        _count = count;
    }

    public override void Reserve()
    {
        _item.ReservedForActivity = true;
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

            if (!_item.IsMapItem)
            {
                throw new Exception("Item must lay on the ground to be picked up.");
            }
            
            var pickedItem = _item.PickUpItem(_count ?? _item.Count);
            pickedItem.SetHolder(_character);
        }
        else
        {
            _item.ReservedForActivity = false;
            IsFinished = true;            
        }
    }
}