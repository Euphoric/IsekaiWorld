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
            var activityList = Game.Jobs.GetJobActivity(_character)?.ToList() ?? new List<Activity>();

            if (activityList.Any())
            {
                foreach (var activity in activityList)
                {
                    activity.Reserve();
                }

                ActivityPlan = activityList;
                IsFinished = true;
            }
        }
        
        _planningDelay = (_planningDelay + 1) % GameSpeed.BaseTps;
    }

    public List<Activity>? ActivityPlan { get; private set; }
}