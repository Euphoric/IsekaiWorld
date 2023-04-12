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
        bool isNextToEntity = Character.Position.IsNextTo(Construction.Position);
        if (!isNextToEntity)
        {
            throw new Exception("TODO Handle case when activity is not in neighbor of target entity.");
        }

        Construction.AddProgress(1);

        if (Construction.IsFinished)
        {
            IsFinished = true;
            Construction.RemoveEntity();
            if (Construction.Definition.PlaceBuildingId != null)
            {
                var buildingDefinition = BuildingDefinitions.GetById(Construction.Definition.PlaceBuildingId);
                Game.SpawnBuilding(Construction.Position, Construction.Rotation, buildingDefinition);
            }
            else if (Construction.Definition.PlaceFloorId != null)
            {
                // TODO
            }
            else
            {
                throw new Exception("Invalid construction setup for: " + Construction.Definition.Id);
            }
        }
    }
}