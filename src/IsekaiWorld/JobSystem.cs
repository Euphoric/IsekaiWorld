using System;
using System.Collections.Generic;
using System.Linq;

public interface IJob
{
    Boolean InProgress { get; }

    void StartWorking(CharacterEntity character);
}

public class JobSystem
{
    private readonly List<IJob> _jobs = new List<IJob>();
    
    public IJob GetNextJob(CharacterEntity character)
    {
        var availableJobs = _jobs.Where(o => !o.InProgress).ToList();
        if (!availableJobs.Any())
            return null;

        var job = availableJobs.First();
        return job;
    }

    public void Add(IJob job)
    {
        _jobs.Add(job);
    }
}