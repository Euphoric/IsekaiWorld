using System.Collections.Generic;

namespace IsekaiWorld;

public interface IJobGiver
{
    IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character);
}

public class JobSystem : IJobGiver
{
    private readonly IReadOnlyList<IJobGiver> _jobGivers;

    public JobSystem(IReadOnlyList<IJobGiver> jobGivers)
    {
        _jobGivers = jobGivers;
    }

    public IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character)
    {
        foreach (var jobGiver in _jobGivers)
        {
            var jobs = jobGiver.GetJobActivity(character);
            if (jobs != null)
                return jobs;
        }

        return null;
    }
}