using System;
using System.Collections.Generic;
using System.Linq;

public class CutWoodJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public CutWoodJobGiver(GameEntity game)
    {
        _game = game;
    }

    public bool SetJobActivity(CharacterEntity character)
    {
        var treesToCut = _game.Buildings.Where(x => x.Definition == BuildingDefinitions.TreeOak && x.Designation == "CutWood");
        
        var tree = treesToCut.FirstOrDefault();
        if (tree == null)
            return false;
        
        character.StartActivity(new CutTreeActivity(_game, character, tree));
        return true;
    }
}