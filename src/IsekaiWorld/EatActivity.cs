using System.Linq;

public class EatActivity : Activity
{
    private readonly CharacterEntity _character;

    public EatActivity(GameEntity game, CharacterEntity character) : base(game)
    {
        _character = character;
    }

    protected override void UpdateInner()
    {
        _character.Hunger = 1;
        var foodItem = Game.Items.First(x => x.Definition == ItemDefinitions.Grains);
        //TODO: Remove item correctly by sending message
        foodItem.Remove();

        IsFinished = true;
    }
}