using System;

namespace IsekaiWorld;

public class GatherActivity : Activity
{
    public CharacterEntity Character { get; }
    public BuildingEntity EntityToGather { get; }

    public GatherActivity(GameEntity game, CharacterEntity character, BuildingEntity entityToGather)
        :base(game)
    {
        Character = character;
        EntityToGather = entityToGather;
    }

    public override void Reserve()
    {
        EntityToGather.ReservedForActivity = true;
    }

    protected override void UpdateInner()
    {
        bool isNextToEntity =
            Character.Position == EntityToGather.Position ||
            Character.Position.IsNextTo(EntityToGather.Position);
        if (!isNextToEntity)
        {
            throw new Exception("TODO Handle case when activity is not in neighbor of target entity.");
        }
        
        if (!EntityToGather.IsRemoved)
        {
            EntityToGather.RemoveEntity();
            var dropItem = EntityToGather.Definition.GatherDrop;
            if (dropItem != null)
            {
                Game.SpawnItem(EntityToGather.Position, dropItem, 1);
            }
        }
        else
        {
            IsFinished = true;
        }
    }
}