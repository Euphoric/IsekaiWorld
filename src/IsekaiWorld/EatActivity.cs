using System;
using System.Linq;

namespace IsekaiWorld;

public class EatActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _foodItem;

    private bool _eating = false;
    
    public EatActivity(GameEntity game, CharacterEntity character, ItemEntity foodItem) : base(game)
    {
        _character = character;
        _foodItem = foodItem;
    }

    protected override void UpdateInner()
    {
        var carriedFood = _character.CarriedItems.FirstOrDefault(x => x.Definition == _foodItem.Definition && x.Count == 1);
        if (carriedFood == null)
        {
            throw new Exception("TODO Handle case when not carrying necessary food");
        }

        if (_eating)
        {
            IsFinished = true;
        }
        else
        {
            _eating = true;
            
            _character.Hunger = 1;
            carriedFood.Remove();
        }
    }
}