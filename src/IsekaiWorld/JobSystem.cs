using System.Collections.Generic;

public interface IJobGiver
{
    bool SetJobActivity(CharacterEntity character);
}

public class JobSystem : IJobGiver
{
    private readonly IReadOnlyList<IJobGiver> _jobGivers;

    public JobSystem(IReadOnlyList<IJobGiver> jobGivers)
    {
        _jobGivers = jobGivers;
    }

    public bool SetJobActivity(CharacterEntity character)
    {
        foreach (var jobGiver in _jobGivers)
        {
            if (jobGiver.SetJobActivity(character))
                return true;
        }

        return false;
    }
}