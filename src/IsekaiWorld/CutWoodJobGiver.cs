using System.Collections.Generic;
using System.Linq;

public class CutWoodJobGiver : IJobGiver
{
    private readonly GameEntity _game;
    private readonly List<BuildingEntity> _treesToCut = new List<BuildingEntity>();
    
    public CutWoodJobGiver(GameEntity game)
    {
        _game = game;
    }

    public void CutTree(BuildingEntity tree)
    {
        _treesToCut.Add(tree);
    }
    
    public bool SetJobActivity(CharacterEntity character)
    {
        var tree = _treesToCut.FirstOrDefault();
        if (tree == null)
            return false;

        _treesToCut.Remove(tree);
        character.StartActivity(new CutTreeActivity(_game, character, tree));
        return true;
    }
}