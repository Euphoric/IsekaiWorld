using System.Collections.Generic;
using System.Linq;

public class CharacterEntity
{
    private readonly GameEntity _game;

    public HexCubeCoord Position { get; set; }
    public bool IsIdle => CurrentActivity == null;

    public IActivity CurrentActivity { get; private set; }

    public CharacterEntity(GameEntity game)
    {
        _game = game;
    }
    
    public INodeOperation Initialize()
    {
        return new CreateCharacter(this);
    }

    public INodeOperation Update(float delta)
    {
        if (CurrentActivity != null && CurrentActivity.IsFinished)
        {
            CurrentActivity = null;
        }
        
        if (CurrentActivity == null)
        {
            var job = _game.GetNextJob(this);
            if (job != null)
            {
                job.StartWorking(this);
                _game.RunActivity(CurrentActivity);
            }
        }

        return new UpdateCharacter(this);
    }

    public void StartActivity(IActivity activity)
    {
        CurrentActivity = activity;
    }
}