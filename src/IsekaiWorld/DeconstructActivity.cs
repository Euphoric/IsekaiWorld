using System;

namespace IsekaiWorld;

public class DeconstructActivity : Activity
{
    public CharacterEntity Character { get; }
    public BuildingEntity Building { get; }

    private bool _hasStarted;
    
    public DeconstructActivity(GameEntity game, CharacterEntity character, BuildingEntity building)
        :base(game)
    {
        Character = character;
        Building = building;
    }

    protected override void UpdateInner()
    {
        var canWork = Character.Position.IsNextTo(Building.Position);
        if (!canWork)
        {
            throw new Exception("Cannot work when not next to.");
        }

        if (_hasStarted)
        {
            Building.RemoveEntity();
            IsFinished = true;
        }
        else
        {
            _hasStarted = true;
        }
    }
}