using System;

namespace IsekaiWorld.Game;

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

    public override void Reserve()
    {
        Building.ReservedForActivity = true;
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
            Building.Remove();
            IsFinished = true;
        }
        else
        {
            _hasStarted = true;
        }
    }
}