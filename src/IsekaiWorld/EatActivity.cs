using System.Linq;

public class EatActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _foodItem;

    private bool _waitUpdate = true;
    
    public EatActivity(GameEntity game, CharacterEntity character, ItemEntity foodItem) : base(game)
    {
        _character = character;
        _foodItem = foodItem;
    }

    protected override void UpdateInner()
    {
        if (_waitUpdate)
        {
            _waitUpdate = false;
            return;
        }
        
        _character.Hunger = 1;
        var pickedItem = _foodItem.PickUpItem(1);
        pickedItem.Remove();
        IsFinished = true;      
    }
}