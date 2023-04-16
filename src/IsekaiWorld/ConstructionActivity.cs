using System;

namespace IsekaiWorld;

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

    protected override void UpdateInner()
    {
        bool isNextToEntity =
            Character.Position == Construction.Position ||
            Character.Position.IsNextTo(Construction.Position);
        if (!isNextToEntity)
        {
            throw new Exception("TODO Handle case when activity is not in neighbor of target entity.");
        }

        Construction.AddProgress(1);

        if (Construction.IsFinished)
        {
            IsFinished = true;
            Construction.RemoveEntity();
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