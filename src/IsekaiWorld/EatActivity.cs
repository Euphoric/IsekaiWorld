using System.Linq;
using System.Net.Http;

public class EatActivity : Activity
{
    private readonly CharacterEntity _character;
    private readonly ItemEntity _foodItem;

    private PickUpItemActivity? _pickUpItemActivity;
    private ItemEntity? _carriedFood;
    
    public EatActivity(GameEntity game, CharacterEntity character, ItemEntity foodItem) : base(game)
    {
        _character = character;
        _foodItem = foodItem;
    }

    protected override void UpdateInner()
    {
        if (_carriedFood == null)
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
                    _carriedFood = _pickUpItemActivity.PickedUpItem;
                    _pickUpItemActivity = null;
                }
            }
        }
        else
        {
            _character.Hunger = 1;
            _carriedFood.Remove();
            IsFinished = true;            
        }
    }
}