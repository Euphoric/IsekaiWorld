using System;

namespace IsekaiWorld;

public class CutTreeActivity : Activity
{
    public CharacterEntity Character { get; }
    public BuildingEntity Tree { get; }

    public CutTreeActivity(GameEntity game, CharacterEntity character, BuildingEntity tree)
        : base(game)
    {
        Character = character;
        Tree = tree;
    }

    public override void Reserve()
    {
        Tree.ReservedForActivity = true;
    }

    protected override void UpdateInner()
    {
        bool isNextToEntity = Character.Position.IsNextTo(Tree.Position);
        if (!isNextToEntity)
        {
            throw new Exception("TODO Handle case when activity is not in neighbor of target entity.");
        }
        
        if (!Tree.IsRemoved)
        {
            Tree.Remove();
            Game.SpawnItem(Tree.Position, ItemDefinitions.Wood, 5);
        }
        else
        {
            IsFinished = true;
        }
    }
}