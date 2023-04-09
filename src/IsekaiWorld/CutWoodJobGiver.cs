using System.Linq;

namespace IsekaiWorld;

public class CutWoodJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public CutWoodJobGiver(GameEntity game)
    {
        _game = game;
    }

    public bool SetJobActivity(CharacterEntity character)
    {
        var treesToCut = _game.Buildings.Where(x => x.Definition == BuildingDefinitions.Plant.TreeOak && x.Designation == DesignationDefinitions.CutWood);
        
        var tree = treesToCut.FirstOrDefault();
        if (tree == null)
            return false;
        
        character.StartActivity(new CutTreeActivity(_game, character, tree));
        return true;
    }
}