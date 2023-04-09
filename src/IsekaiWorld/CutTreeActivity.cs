using System.Linq;

public class CutTreeActivity : Activity
{
    public CharacterEntity Character { get; }
    public BuildingEntity Tree { get; }

    private MovementActivity? _movement;

    public CutTreeActivity(GameEntity game, CharacterEntity character, BuildingEntity tree)
        : base(game)
    {
        Character = character;
        Tree = tree;
    }

    protected override void UpdateInner()
    {
        if (_movement != null)
        {
            _movement.Update();
        }

        var canCut = Character.Position.IsNextTo(Tree.Position);
        if (!canCut)
        {
            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(Game, Game.Pathfinding, Character, Tree.Position, true);
            }
        }
        else
        {
            _movement = null;

            if (!Tree.IsRemoved)
            {
                Tree.RemoveEntity();
                Game.SpawnItem(Tree.Position, ItemDefinitions.Wood, 5);
            }
            else
            {
                IsFinished = true;
            }
        }
    }
}