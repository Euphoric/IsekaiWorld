using System.Collections.Generic;

public class CharacterEntity : IEntity
{
    public bool IsRemoved => false;
    
    private readonly GameEntity _game;

    public HexCubeCoord Position { get; set; }
    public bool IsIdle => CurrentActivity == null;

    public IActivity CurrentActivity { get; private set; }
    public string Label { get; private set; }

    public CharacterEntity(GameEntity game, string label)
    {
        Label = label;
        _game = game;
    }
    
    public INodeOperation Initialize()
    {
        return new CreateCharacter(this);
    }

    public IEnumerable<INodeOperation> Update()
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

        yield return new UpdateCharacter(this);
    }

    public void StartActivity(IActivity activity)
    {
        CurrentActivity = activity;
    }
}