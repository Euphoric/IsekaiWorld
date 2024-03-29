using System;
using System.Linq;

namespace IsekaiWorld.Game;

public class DropItemActivity : Activity
{
    private CharacterEntity Character { get; }
    private ItemEntity Item { get; }
    private bool _executing;

    private readonly BuildingEntity _targetStockpile;

    public DropItemActivity(GameEntity game, CharacterEntity character, ItemEntity item, BuildingEntity targetStockpile)
        : base(game)
    {
        _targetStockpile = targetStockpile;
        Character = character;
        Item = item;
    }

    public override void Reserve()
    {
        _targetStockpile.ReservedForActivity = true;
    }

    protected override void UpdateInner()
    {
        if (!_executing)
        {
            _executing = true;

            if (!Character.CarriedItems.Contains(Item))
            {
                throw new Exception("Abort because characters is not carrying the item to drop");
            }

            var itemInPlace = Game.MapItems
                .FirstOrDefault(x => x.Position == _targetStockpile.Position && x.Definition == Item.Definition);
            if (itemInPlace == null)
            {
                // place item on ground
                Item.Position = _targetStockpile.Position;
                Item.SetHolder(Game.MapItemsHolder);
            }
            else
            {
                // stack items together
                itemInPlace.AddCount(Item.Count);
                Item.Remove();
            }
        }
        else
        {
            IsFinished = true;
            _targetStockpile.ReservedForActivity = false;
        }
    }
}