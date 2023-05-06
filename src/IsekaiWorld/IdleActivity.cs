using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

/// <summary>
/// Activity where character is idling or planning next action.
/// </summary>
public class IdleActivity : Activity
{
    private readonly CharacterEntity _character;

    private int _planningDelay;

    public IdleActivity(GameEntity game, CharacterEntity character) : base(game)
    {
        _character = character;
    }

    protected override void UpdateInner()
    {
        if (_planningDelay == 0)
        {
            var activityList = Game.Jobs.BuildPlan(_character);

            if (activityList != null)
            {
                foreach (var activity in activityList.Activities)
                {
                    activity.Reserve();
                }

                ActivityPlan = activityList;
                IsFinished = true;
            }
        }
        
        _planningDelay = (_planningDelay + 1) % GameSpeed.BaseTps;
    }

    public ActivityPlan? ActivityPlan { get; private set; }
}