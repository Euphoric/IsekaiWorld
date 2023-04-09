using System;

namespace IsekaiWorld;

public abstract class Activity
{
    public Guid Id { get; } = Guid.NewGuid();
    
    protected GameEntity Game { get; }

    protected Activity(GameEntity game)
    {
        Game = game;
    }
    
    public bool IsFinished { get; protected set; }
    
    public void Update()
    {
        if (Game.Paused)
            return;

        if (IsFinished)
            return;
        
        UpdateInner();
    }

    protected abstract void UpdateInner();
}