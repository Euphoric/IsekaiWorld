namespace IsekaiWorld;

public class IdleActivity : Activity
{
    public IdleActivity(GameEntity game) : base(game)
    {
        IsFinished = true;
    }

    protected override void UpdateInner()
    { }
}