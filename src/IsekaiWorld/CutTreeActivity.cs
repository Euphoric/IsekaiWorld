using System.Linq;

public class CutTreeActivity : IActivity
{
    private readonly GameEntity _game;
    public CharacterEntity Character { get; }
    public BuildingEntity Tree { get; }

    private MovementActivity _movement;
    
    public bool IsFinished { get; private set; }

    public CutTreeActivity(GameEntity game, CharacterEntity character, BuildingEntity tree)
    {
        _game = game;
        Character = character;
        Tree = tree;
    }

    public void Update(float delta)
    {
        if (IsFinished)
            return;

        if (_movement != null)
        {
            _movement.Update(delta);
        }
        
        var canCut = Character.Position.IsNextTo(Tree.Position);
        if (canCut)
        {
            _movement = null;

            Tree.RemoveEntity();
            _game.SpawnItem(Tree.Position, ItemDefinitions.Wood);
            
            IsFinished = true;
        }
        else
        {

            if (_movement == null)
            {
                // move on item
                _movement = new MovementActivity(_game.Pathfinding, Character, Tree.Position, true);
            }
        }
    }
}