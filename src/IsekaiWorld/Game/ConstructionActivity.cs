using System;

namespace IsekaiWorld.Game;

public class ConstructionActivity : Activity
{
    public CharacterEntity Character { get; }
    public ConstructionEntity Construction { get; }

    public ConstructionActivity(GameEntity game, CharacterEntity character, ConstructionEntity construction)
        :base(game)
    {
        Character = character;
        Construction = construction;
    }

    public override void Reserve()
    {
        Construction.ReservedForActivity = true;
    }

    protected override void UpdateInner()
    {
        bool isNextToEntity = Character.Position.IsNextTo(Construction.Position);
        if (!isNextToEntity)
        {
            throw new Exception("TODO Handle case when activity is not in neighbor of target entity.");
        }

        if (!Construction.IsFinished)
        {
            Construction.AddProgress(GameSpeed.TimePerTick);
        }
        else
        {
            IsFinished = true;
            Construction.Remove();
            if (Construction.Definition.PlaceBuilding != null)
            {
                Game.SpawnBuilding(Construction.Position, Construction.Rotation, Construction.Definition.PlaceBuilding);
            }
            else if (Construction.Definition.PlaceFloor != null)
            {
                Game.SetFloor(Construction.Position, Construction.Definition.PlaceFloor);
            }
            else
            {
                throw new Exception("Invalid construction setup for: " + Construction.Definition.Id);
            }
        }
    }
}