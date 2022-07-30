using System;
using System.Collections.Generic;
using System.Linq;

[Obsolete("Use IJobGiver")]
public interface IJob
{
    Boolean InProgress { get; }

    void StartWorking(CharacterEntity character);
}

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

    private readonly List<IJob> _jobs = new List<IJob>();

    public void Add(IJob job)
    {
        _jobs.Add(job);
    }

    public bool SetJobActivity(CharacterEntity character)
    {
        foreach (var jobGiver in _jobGivers)
        {
            if (jobGiver.SetJobActivity(character))
                return true;
        }

        var availableJobs = _jobs.Where(o => !o.InProgress).ToList();
        if (availableJobs.Any())
        {
            availableJobs.First().StartWorking(character);
            return true;
        }

        return false;
    }
}