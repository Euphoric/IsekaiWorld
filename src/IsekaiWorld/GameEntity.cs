using System.Collections.Generic;
using System.Linq;

public class GameEntity
{
    public HexMap GameMap { get; private set; }
    public HexagonPathfinding Pathfinding { get; private set; }

    public GameUserInterface UserInterface { get; private set; }

    public IReadOnlyList<ConstructionEntity> Constructions => _constructionEntities;
    public IReadOnlyList<BuildingEntity> Buildings => _buildings;

    private readonly List<CharacterEntity> _characters = new List<CharacterEntity>();
    private readonly List<BuildingEntity> _buildings = new List<BuildingEntity>();
    
    private readonly List<ConstructionEntity> _constructionEntities = new List<ConstructionEntity>();
    private readonly List<ConstructionJob> _constructionJobs = new List<ConstructionJob>();

    private readonly List<IActivity> _activities = new List<IActivity>();

    private readonly List<INodeOperation> _operations = new List<INodeOperation>();

    public void Initialize(IMapGenerator mapGenerator)
    {
        UserInterface = new GameUserInterface(this);

        GameMap = mapGenerator.GenerateNewMap();

        Pathfinding = new HexagonPathfinding();
        Pathfinding.BuildMap(GameMap);
    }

    public CharacterEntity AddCharacter(string label)
    {
        var characterEntity = new CharacterEntity(this, label)
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

        foreach (var operation in UserInterface.Update())
        {
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

        GameMap.Update(map);
    }

    public void StartConstruction(HexCubeCoord position)
    {
        var constructionExists = _constructionEntities.Any(x => x.Position == position);
        var isTerrainPassable = GameMap.CellForPosition(position).Surface.IsPassable;
        if (!constructionExists && isTerrainPassable)
        {
            var constructionEntity = new ConstructionEntity(position, BuildingDefinitions.Wall);
            _constructionEntities.Add(constructionEntity);

            _operations.Add(new AddConstructionOperation(constructionEntity));

            _constructionJobs.Add(new ConstructionJob(this, constructionEntity));
        }
    }

    public ConstructionJob GetNextJob(CharacterEntity character)
    {
        var availableJobs = _constructionJobs.Where(o => !o.InProgress).ToList();
        if (!availableJobs.Any())
            return null;

        var job = availableJobs.First();
        return job;
    }

    public CharacterEntity CharacterOn(HexCubeCoord position)
    {
        return _characters.FirstOrDefault(c => c.Position == position);
    }
    
    public BuildingEntity BuildingOn(HexCubeCoord position)
    {
        return _buildings.FirstOrDefault(b => b.Position == position);
    }
    

    public void SpawnBuilding(ConstructionEntity construction)
    {
        var surface = construction.BuildingDefinition.Surface;
        var position = construction.Position;
        
        _buildings.Add(new BuildingEntity(position, construction.BuildingDefinition));
        GameMap.SetCell(position, surface);
        Pathfinding.SetPathing(position, surface);

        var stuckCharacter = _characters.FirstOrDefault(c => c.Position == position);
        if (stuckCharacter != null)
        {
            var unstuckCell = stuckCharacter.Position.Neighbors().Select(c => GameMap.CellForPosition(c))
                .FirstOrDefault(c => c.Surface.IsPassable);
            if (unstuckCell != null)
            {
                stuckCharacter.Position = unstuckCell.Position;
            }
        }
    }

    public IEnumerable<string> CheckForIssues()
    {
        foreach (var character in _characters)
        {
            var cellUnderCharacter = GameMap.CellForPosition(character.Position);
            if (!cellUnderCharacter.Surface.IsPassable)
            {
                yield return $"Character '{character.Label}' stuck on impassable surface on {character.Position}";
            }
        }

        foreach (var construction in _constructionEntities)
        {
            var cellUnderConstruction = GameMap.CellForPosition(construction.Position);
            if (!cellUnderConstruction.Surface.IsPassable)
            {
                yield return $"Construction on impassable surface on {construction.Position}";
            }
        }
    }
}