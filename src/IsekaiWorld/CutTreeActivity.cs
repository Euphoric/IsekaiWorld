using System.Linq;

public class CutTreeActivity : Activity
{
    public CharacterEntity Character { get; }
    public BuildingEntity Tree { get; }

    private MovementActivity? _movement;

    public CutTreeActivity(GameEntity game, CharacterEntity character, BuildingEntity tree)
        :base(game)
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
        if (canCut)
        {
            _movement = null;

            Tree.RemoveEntity();
            Game.SpawnItem(Tree.Position, ItemDefinitions.Wood, 5);
            
            IsFinished = true;
        }
        else
        {

            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(Game, Game.Pathfinding, Character, Tree.Position, true);
            }
        }
    }
}