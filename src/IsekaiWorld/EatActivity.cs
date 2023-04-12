using System.Linq;

namespace IsekaiWorld;

public class EatActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _foodItem;

    private PickUpItemActivity? _pickUpItemActivity;

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
            if (_pickUpItemActivity == null)
            {
                _pickUpItemActivity = new PickUpItemActivity(Game, _character, _foodItem);
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
            _character.Hunger = 1;
            carriedFood.Remove();
            IsFinished = true;            
        }
    }
}