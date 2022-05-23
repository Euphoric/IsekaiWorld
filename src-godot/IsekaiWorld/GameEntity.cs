using System.Collections.Generic;
using System.Linq;

public class GameEntity
{
    public HexMap GameMap { get; private set; }
    public HexagonPathfinding Pathfinding { get; private set; }
    
    private readonly List<CharacterEntity> _characters = new List<CharacterEntity>();
    private readonly List<ConstructionEntity> _constructionEntities = new List<ConstructionEntity>();
    private readonly List<ConstructionJob> _constructionJobs = new List<ConstructionJob>();
    
    private readonly List<IActivity> _activities = new List<IActivity>();

    private readonly List<INodeOperation> _operations = new List<INodeOperation>();
    
    public void Initialize()
    {
        GameMap = new HexMap(32);
        GameMap.GenerateMap();
        
        Pathfinding = new HexagonPathfinding();
        Pathfinding.BuildMap(GameMap);
    }

    public CharacterEntity AddCharacter()
    {
        var characterEntity = new CharacterEntity(this)
        {
            Position = HexCubeCoord.Zero,
        };

        _characters.Add(characterEntity);

        _operations.Add(characterEntity.Initialize());

        return characterEntity;
    }
    
    public void RunActivity(IActivity activity)
    {
        _activities.Add(activity);
    }
    
    public void RemoveConstruction(ConstructionEntity construction)
    {
        _constructionEntities.Remove(construction);
        _operations.Add(new RemoveConstruction(construction));
    }

    public void Update(float delta)
    {
        foreach (var character in _characters)
        {
            var operation = character.Update(delta);
            _operations.Add(operation);
        }

        foreach (var activity in _activities)
        {
            activity.Update(delta);
        }

        foreach (var entity in _constructionEntities)
        {
            var operation = entity.UpdateNode();
            _operations.Add(operation);
        }

        _activities.RemoveAll(x => x.IsFinished);
    }

    public void UpdateNodes(HexagonalMap map)
    {
        foreach (var operation in _operations)
        {
            operation.Execute(map);
        }
        _operations.Clear();
    }

    public void StartConstruction(HexCubeCoord position)
    {
        var constructionExists = _constructionEntities.Any(x => x.Position == position);
        if (!constructionExists)
        {
            var constructionEntity = new ConstructionEntity
            {
                Position = position
            };
            _constructionEntities.Add(constructionEntity);

            _operations.Add(new AddConstructionOperation(constructionEntity));
            
            _constructionJobs.Add(new ConstructionJob(this, constructionEntity));
        }
    }

    public ConstructionJob GetNextJob(CharacterEntity character)
    {
        var availableJobs = _constructionJobs.Where(o=>!o.InProgress).ToList();
        if (!availableJobs.Any())
            return null;

        var job = availableJobs.First();
        return job;
    }
}