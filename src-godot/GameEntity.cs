using System.Collections.Generic;
using System.Linq;
using Godot;

public class GameEntity
{
    public HexagonalMap Map { get; }
    public HexMap GameMap { get; private set; }

    private HexagonPathfinding _pathfinding;
    private CharaterEntity _characterEntity;
    private readonly List<ConstructionEntity> _constructionEntities = new List<ConstructionEntity>();

    private readonly List<IActivity> _activities = new List<IActivity>();

    public GameEntity(HexagonalMap map)
    {
        Map = map;
    }
    
    public void Initialize()
    {
        GameMap = new HexMap(32);
        GameMap.GenerateMap();
        
        _pathfinding = new HexagonPathfinding();
        _pathfinding.BuildMap(GameMap);
    }

    public void AddCharacter()
    {
        var characterHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Scale = new Vector2(Map.HexSize, Map.HexSize),
            Color = Colors.Blue
        };
        _characterEntity = new CharaterEntity(this, _pathfinding)
        {
            Position = HexCubeCoord.Zero,
            Node = characterHexagon
        };

        Map.AddChild(characterHexagon);
    }
    
    public void RunActivity(IActivity activity)
    {
        _activities.Add(activity);
    }
    
    public void RemoveConstruction(ConstructionEntity construction)
    {
        _constructionEntities.Remove(construction);
        Map.RemoveChild(construction.Node);
    }

    public void Update(float delta)
    {
        if (_characterEntity.IsIdle && _constructionEntities.Any())
        {
            var firstConstruction = _constructionEntities.First();
            _characterEntity.Construct(firstConstruction);
        }

        _characterEntity.Update(delta);

        foreach (var activity in _activities)
        {
            activity.Update(delta);
        }

        foreach (var entity in _constructionEntities)
        {
            entity.UpdateNode();
        }
        
        _activities.RemoveAll(x => x.IsFinished);
    }

    public void StartConstruction(HexCubeCoord position)
    {
        var constructionExists = _constructionEntities.Any(x => x.Position == position);
        if (!constructionExists)
        {
            var constructionEntity = new ConstructionEntity();
            constructionEntity.Position = position;
            _constructionEntities.Add(constructionEntity);

            var constructionNode = new HexagonNode
            {
                Color = Colors.MediumPurple,
                Scale = new Vector2(Map.HexSize, Map.HexSize),
            };
            constructionNode.HexPosition = constructionEntity.Position;
            Map.AddChild(constructionNode);
            constructionEntity.Node = constructionNode;
        }
    }
}