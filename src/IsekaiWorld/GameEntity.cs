using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameEntity
{
    public HexagonalMapEntity GameMap { get; private set; }
    public HexagonPathfinding Pathfinding { get; private set; }
    public GameUserInterface UserInterface { get; private set; }

    public IReadOnlyList<ConstructionEntity> Constructions => _entities.OfType<ConstructionEntity>().ToList();
    public IReadOnlyList<BuildingEntity> Buildings => _entities.OfType<BuildingEntity>().ToList();

    private readonly List<ConstructionJob> _constructionJobs = new List<ConstructionJob>();

    private readonly List<IActivity> _activities = new List<IActivity>();

    private readonly List<INodeOperation> _operations = new List<INodeOperation>();

    private readonly List<IEntity> _entities = new List<IEntity>();
    
    public void Initialize(IMapGenerator mapGenerator)
    {
        UserInterface = new GameUserInterface(this);

        var (map, buildings) = mapGenerator.GenerateNewMap();
        GameMap = map;
        _entities.AddRange(buildings);
        
        Pathfinding = new HexagonPathfinding();
        Pathfinding.BuildMap(GameMap);
    }

    public CharacterEntity AddCharacter(string label)
    {
        var characterEntity = new CharacterEntity(this, label)
        {
            Position = HexCubeCoord.Zero,
        };

        _entities.Add(characterEntity);

        _operations.Add(characterEntity.Initialize());

        return characterEntity;
    }

    public void RunActivity(IActivity activity)
    {
        _activities.Add(activity);
    }

    public void RemoveConstruction(ConstructionEntity construction)
    {
        construction.RemoveEntity();
    }

    public void Update(float delta)
    {
        foreach (var entity in _entities)
        {
            var operations = entity.Update();
            _operations.AddRange(operations);
        }
        _entities.RemoveAll(ent => ent.IsRemoved);

        foreach (var operation in UserInterface.Update())
        {
            _operations.Add(operation);
        }

        foreach (var operation in GameMap.Update())
        {
            _operations.Add(operation);
        }
        
        foreach (var activity in _activities)
        {
            activity.Update(delta);
        }
        _activities.RemoveAll(x => x.IsFinished);
    }

    public void UpdateNodes(GameNode gameNode)
    {
        foreach (var operation in _operations)
        {
            operation.Execute(gameNode);
        }

        _operations.Clear();
    }

    public void StartConstruction(HexCubeCoord position, HexagonDirection rotation, ConstructionDefinition construction)
    {
        var constructionExists = _entities.OfType<ConstructionEntity>().Any(x => x.Position == position);
        var isTerrainPassable = GameMap.CellForPosition(position).Surface.IsPassable;
        if (!constructionExists && isTerrainPassable)
        {
            var constructionEntity = new ConstructionEntity(position, rotation, construction);
            _entities.Add(constructionEntity);
            
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

    public IEnumerable<IEntity> EntitiesOn(HexCubeCoord position)
    {
        return _entities.Where(c => c.OccupiedCells.Contains(position));
    }

    public void SpawnBuilding(HexCubeCoord position, HexagonDirection rotation, BuildingDefinition buildingDefinition)
    {
        _entities.Add(new BuildingEntity(position, rotation, buildingDefinition));
        //Pathfinding.SetPathing(position, surface);

        var stuckCharacter = _entities.OfType<CharacterEntity>().FirstOrDefault(c => c.Position == position);
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
        foreach (var character in _entities.OfType<CharacterEntity>())
        {
            var cellUnderCharacter = GameMap.CellForPosition(character.Position);
            if (!cellUnderCharacter.Surface.IsPassable)
            {
                yield return $"Character '{character.Label}' stuck on impassable surface on {character.Position}";
            }
        }

        foreach (var construction in _entities.OfType<ConstructionEntity>())
        {
            var cellUnderConstruction = GameMap.CellForPosition(construction.Position);
            if (!cellUnderConstruction.Surface.IsPassable)
            {
                yield return $"Construction on impassable surface on {construction.Position}";
            }
        }
    }

    public void SpawnItem(HexCubeCoord position, ItemDefinition item)
    {
        var existingEntity = _entities.OfType<ItemEntity>().FirstOrDefault(i => i.Position == position && i.Definition == item);
        var spawnNewEntity = existingEntity == null;
        if (spawnNewEntity)
        {
            var itemEntity = new ItemEntity(position, item, 1);
            _entities.Add(itemEntity);
        }
        else
        {
            existingEntity.AddCount(1);
        }
    }
}