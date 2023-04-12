using System.Collections.Generic;

namespace IsekaiWorld;

public interface IJobGiver
{
    Activity? GetJobActivity(CharacterEntity character);
}

public class JobSystem : IJobGiver
{
    private readonly IReadOnlyList<IJobGiver> _jobGivers;

    public JobSystem(IReadOnlyList<IJobGiver> jobGivers)
    {
        _jobGivers = jobGivers;
    }

    public Activity? GetJobActivity(CharacterEntity character)
    {
        foreach (var jobGiver in _jobGivers)
        {
            var a = jobGiver.GetJobActivity(character);
            if (a != null)
                return a;
        }

        return null;
    }
}