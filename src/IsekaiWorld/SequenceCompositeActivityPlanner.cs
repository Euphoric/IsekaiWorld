using System.Collections.Generic;

namespace IsekaiWorld;

public class ActivityPlan
{
    public IReadOnlyList<Activity> Activities { get; }

    public ActivityPlan(IReadOnlyList<Activity> activities)
    {
        Activities = activities;
    }
}

public interface IActivityPlanner
{
    ActivityPlan? BuildPlan(CharacterEntity character);
}

public class SequenceCompositeActivityPlanner : IActivityPlanner
{
    private readonly IReadOnlyList<IActivityPlanner> _planners;

    public SequenceCompositeActivityPlanner(IReadOnlyList<IActivityPlanner> planners)
    {
        _planners = planners;
    }

    public ActivityPlan? BuildPlan(CharacterEntity character)
    {
        foreach (var planner in _planners)
        {
            var jobs = planner.BuildPlan(character);
            if (jobs != null)
                return jobs;
        }

        return null;
    }
}