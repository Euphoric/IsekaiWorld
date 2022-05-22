using System.Collections.Generic;
using System.Linq;
using Godot;

public class GameEntity
{
    public HexagonalMap Map { get; }
    public HexMap GameMap { get; private set; }

    private HexagonPathfinding _pathfinding;
    private readonly List<CharaterEntity> _characters = new List<CharaterEntity>();
    private readonly List<ConstructionEntity> _constructionEntities = new List<ConstructionEntity>();
    private readonly List<ConstructionJob> _constructionJobs = new List<ConstructionJob>();
    
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
            Color = Colors.Blue
        };
        var characterEntity = new CharaterEntity(this, _pathfinding)
        {
            Position = HexCubeCoord.Zero,
            Node = characterHexagon
        };

        _characters.Add(characterEntity);
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
        var idleCharacter = _characters.Where(x => x.IsIdle);
        foreach (var character in idleCharacter)
        {
            var availableJobs = _constructionJobs.Where(o=>!o.InProgress).ToList();
            if (!availableJobs.Any())
                break;

            var job = availableJobs.First();
            job.StartWorking(character);
        }

        foreach (var character in _characters)
        {
            character.Update(delta);            
        }

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
            };
            constructionNode.HexPosition = constructionEntity.Position;
            Map.AddChild(constructionNode);
            constructionEntity.Node = constructionNode;
            
            _constructionJobs.Add(new ConstructionJob(constructionEntity));
        }
    }
}