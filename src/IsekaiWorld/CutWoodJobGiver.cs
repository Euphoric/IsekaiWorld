using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class CutWoodJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public CutWoodJobGiver(GameEntity game)
    {
        _game = game;
    }

    public IReadOnlyList<Activity>? GetJobActivity(CharacterEntity character)
    {
        var treesToCut = _game
                .Buildings
                .Where(x => x.Definition == BuildingDefinitions.Plant.TreeOak && x.Designation == DesignationDefinitions.CutWood)
                .Where(x => !x.ReservedForActivity)
            ;

        var tree = treesToCut.FirstOrDefault();
        if (tree == null)
            return null;

        return new Activity[]
        {
            new MovementActivity(_game, _game.Pathfinding, character, tree.Position.Neighbors()),
            new CutTreeActivity(_game, character, tree)
        };
    }
}