using System;
using System.Linq;

namespace IsekaiWorld;

public class DeliverItemActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _item;
    private readonly ConstructionEntity _construction;

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
            throw new Exception("TODO Not carrying necessary items");
        }
        
        bool isNextToEntity =
            _character.Position == _construction.Position ||
            _character.Position.IsNextTo(_construction.Position);
        if (!isNextToEntity)
        {
            throw new Exception("TODO Handle case when activity is not in neighbor of target entity.");
        }

        (_character as IItemHolder).RemoveItem(carriedItem); // TODO: Remove carried item when item is removed?
        carriedItem.Remove();

        _construction.MaterialsDelivered = true;

        IsFinished = true;
    }
}