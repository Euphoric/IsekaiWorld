using System.Collections.Generic;
using System.Linq;

public class HaulJobGiver : IJobGiver
{
    private readonly GameEntity _game;
    private readonly List<ItemEntity> _itemsToHaul = new List<ItemEntity>();

    public HaulJobGiver(GameEntity game)
    {
        _game = game;
    }

    public bool SetJobActivity(CharacterEntity character)
    {
        var itemToHaul = _itemsToHaul.FirstOrDefault();
        if (itemToHaul == null) 
            return false;

        _itemsToHaul.Remove(itemToHaul);
        character.StartActivity(new HaulItemActivity(_game, character, itemToHaul));
        return true;

    }

    public void HaulItem(ItemEntity item)
    {
        _itemsToHaul.Add(item);
    }
}