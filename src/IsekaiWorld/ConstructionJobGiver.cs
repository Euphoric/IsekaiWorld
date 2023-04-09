using System.Linq;

namespace IsekaiWorld;

public class ConstructionJobGiver : IJobGiver
{
    private readonly GameEntity _game;

    public ConstructionJobGiver(GameEntity game)
    {
        _game = game;
    }
    
    public bool SetJobActivity(CharacterEntity character)
    {
        var construction = _game.Constructions.FirstOrDefault();
        if (construction == null)
            return false;

        if (construction.Definition.Material != null && !construction.MaterialsDelivered)
        {
            var itemToDeliver = _game.Items.FirstOrDefault(x => x.Definition == construction.Definition.Material);
            if (itemToDeliver == null)
                return false;
            
            character.StartActivity(new DeliverItemActivity(_game, character, itemToDeliver, construction));
            return true;
        }
        else
        {
            character.StartActivity(new ConstructionActivity(_game, character, construction));
            return true;
        }
    }
}