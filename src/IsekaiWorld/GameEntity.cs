using System.Collections.Generic;
using System.Linq;

public class GameEntity
{
    public HexagonalMapEntity GameMap { get; private set; }
    public HexagonPathfinding Pathfinding { get; private set; }
    public GameUserInterface UserInterface { get; private set; }
    public JobSystem Jobs { get; private set; }
    public MessagingHub Messaging { get; }
    
    public IReadOnlyList<ConstructionEntity> Constructions => _entities.OfType<ConstructionEntity>().ToList();
    public IReadOnlyList<BuildingEntity> Buildings => _entities.OfType<BuildingEntity>().ToList();
    public IReadOnlyList<ItemEntity> Items => _entities.OfType<ItemEntity>().ToList();
    
    private readonly List<IActivity> _activities = new List<IActivity>();

    private readonly List<INodeOperation> _operations = new List<INodeOperation>();

    private readonly List<IEntity> _entities = new List<IEntity>();

    public MapItems MapItems { get; } = new MapItems();

    public GameEntity()
    {
        Messaging = new MessagingHub();
    }

    public void Initialize(IMapGenerator mapGenerator)
    {
        UserInterface = new GameUserInterface(this);

        var (map, entities) = mapGenerator.GenerateNewMap();
        GameMap = map;
        entities.ForEach(AddEntity);

        Pathfinding = new HexagonPathfinding();
        Pathfinding.BuildMap(GameMap);
        Messaging.Register(Pathfinding.Messaging);
        
        Jobs = new JobSystem();
    }

    public CharacterEntity AddCharacter(string label)
    {
        var characterEntity = new CharacterEntity(this, label)
        {
            Position = HexCubeCoord.Zero,
        };

        AddEntity(characterEntity);

        _operations.Add(characterEntity.Initialize());

        return characterEntity;
    }

    public void RunActivity(IActivity activity)
    {
        _activities.Add(activity);
    }

    public void Update()
    {
        Pathfinding.Update();
        foreach (var entity in _entities)
        {
            var operations = entity.Update();
            _operations.AddRange(operations);
        }
        _entities.Where(ent => ent.IsRemoved).ToList().ForEach(RemoveEntity);
        
        foreach (var operation in UserInterface.Update())
        {
            _operations.Add(operation);
        }

        foreach (var activity in _activities)
        {
            activity.Update();
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
            AddEntity(constructionEntity);

            Jobs.Add(new ConstructionJob(this, constructionEntity));
        }
    }

    public IReadOnlyList<IEntity> EntitiesOn(HexCubeCoord position)
    {
        return _entities.Where(c => c.OccupiedCells.Contains(position)).ToList();
    }

    public void SpawnBuilding(HexCubeCoord position, HexagonDirection rotation, BuildingDefinition buildingDefinition)
    {
        AddEntity(new BuildingEntity(position, rotation, buildingDefinition));

        var stuckCharacter = _entities.OfType<CharacterEntity>().FirstOrDefault(c => c.Position == position);
        if (stuckCharacter != null)
        {
            var unstuckCell = stuckCharacter.Position.Neighbors().Where(p=>Pathfinding.IsPassable(p)).Select(c => GameMap.CellForPosition(c)).FirstOrDefault();
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
            var characterPosition = character.Position;
            var isImpassable = !Pathfinding.IsPassable(characterPosition);
            if (isImpassable)
            {
                yield return $"Character '{character.Label}' stuck on impassable surface on {characterPosition}";
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
            itemEntity.SetHolder(MapItems);
            AddEntity(itemEntity);

            Jobs.Add(new HaulItemJob(this, itemEntity));
        }
        else
        {
            existingEntity.AddCount(1);
        }
    }
    
    public void AddEntity(IEntity entity)
    {
        _entities.Add(entity);
        Messaging.Register(entity.Messaging);
    }
    
    public void RemoveEntity(IEntity entity)
    {
        _entities.Remove(entity);
        Messaging.Unregister(entity.Messaging);
    }

    public void DesignateCutWood(HexCubeCoord position)
    {
        var treeEntity = Buildings.FirstOrDefault(e => e.OccupiedCells.Contains(position) && e.Definition == BuildingDefinitions.TreeOak);
        if (treeEntity != null)
        {
            Jobs.Add(new CutWoodJob(this, treeEntity));
        }
    }
}