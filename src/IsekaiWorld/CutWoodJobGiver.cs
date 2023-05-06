using System.Linq;

namespace IsekaiWorld;

public class CutWoodActivityPlanner : IActivityPlanner
{
    private readonly GameEntity _game;

    public CutWoodActivityPlanner(GameEntity game)
    {
        _game = game;
    }

    public ActivityPlan? BuildPlan(CharacterEntity character)
    {
        var treesToCut = _game
                .Buildings
                .Where(x => x.Definition == BuildingDefinitions.Plant.TreeOak &&
                            x.Designation == DesignationDefinitions.CutWood)
                .Where(x => !x.ReservedForActivity)
            ;

        var tree = treesToCut.FirstOrDefault();
        if (tree == null)
            return null;

        return new ActivityPlan(
            new Activity[]
            {
                new MovementActivity(_game, _game.Pathfinding, character, tree.Position.Neighbors()),
                new CutTreeActivity(_game, character, tree)
            }
        );
    }
}