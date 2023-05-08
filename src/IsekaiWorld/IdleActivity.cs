using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

/// <summary>
/// Activity that might return an activity plan after it is finished.
/// </summary>
public interface IPlanningActivity
{
    ActivityPlan? ActivityPlan { get; }
}

public class ThinkingActivity : Activity, IPlanningActivity
{
    private readonly CharacterEntity _character;

    public ThinkingActivity(GameEntity game, CharacterEntity character) : base(game)
    {
        _character = character;
    }

    protected override void UpdateInner()
    {
        var activityList = Game.Jobs.BuildPlan(_character);

        if (activityList != null)
        {
            foreach (var activity in activityList.Activities)
            {
                activity.Reserve();
            }

            ActivityPlan = activityList;
        }
        else
        {
            ActivityPlan = new ActivityPlan(new Activity[]
            {
                new IdleActivity(Game, _character)
            });
        }
        
        IsFinished = true;
    }
    
    public ActivityPlan? ActivityPlan { get; private set; }
}

/// <summary>
/// Activity where character is idling or planning next action.
/// </summary>
public class IdleActivity : Activity, IPlanningActivity
{
    private readonly CharacterEntity _character;

    private int _planningDelay;
    private ThinkingActivity? _thinkingActivity;

    public IdleActivity(GameEntity game, CharacterEntity character) : base(game)
    {
        _character = character;
    }

    protected override void UpdateInner()
    {
        if (_thinkingActivity == null)
        {
            _planningDelay += 1;
            if (_planningDelay == GameSpeed.BaseTps)
            {
                _thinkingActivity = new ThinkingActivity(Game, _character);
            }
        }
        else
        {
            _thinkingActivity.Update();
            
            if (_thinkingActivity.IsFinished)
            {
                ActivityPlan = _thinkingActivity.ActivityPlan;
                IsFinished = true;

                _thinkingActivity = null;
            }
        }
    }

    public ActivityPlan? ActivityPlan { get; private set; }
}

/// <summary>
/// Activity that does nothing and is immediately finished.
/// </summary>
public class FinishedActivity : Activity
{
    public FinishedActivity(GameEntity game) : base(game)
    {
        IsFinished = true;
    }

    protected override void UpdateInner()
    { }
}