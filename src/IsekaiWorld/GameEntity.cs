using System;
using System.Collections.Generic;
using System.Linq;

public class GameEntity
{
    public HexagonalMapEntity GameMap { get; private set; } = null!;
    public HexagonPathfinding Pathfinding { get; private set; } = null!;
    public GameUserInterface UserInterface { get; private set; }
    public JobSystem Jobs { get; }
    public HaulJobGiver HaulJobGiver { get; }
    public MessagingHub Messaging { get; }

    public IReadOnlyList<ConstructionEntity> Constructions => _entities.OfType<ConstructionEntity>().ToList();
    public IReadOnlyList<BuildingEntity> Buildings => _entities.OfType<BuildingEntity>().ToList();
    public IReadOnlyList<ItemEntity> Items => _entities.OfType<ItemEntity>().ToList();

    private readonly List<IEntity> _entities = new();
    private int _speed;
    public int Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            Messaging.Broadcast(new SpeedChanged(_paused, _speed));
        }
    }

    private Boolean _paused;
    public Boolean Paused
    {
        get => _paused;
        set
        {
            _paused = value;
            Messaging.Broadcast(new SpeedChanged(_paused, _speed));
        }
    }

    public MapItems MapItems { get; } = new();

    public GameEntity()
    {
        Messaging = new MessagingHub();
        UserInterface = new GameUserInterface(this);
        HaulJobGiver = new HaulJobGiver(this);
        var deconstructJobGiver = new DeconstructJobGiver(this);
        var constructionJobGiver = new ConstructionJobGiver(this);
        var cutWoodJobGiver = new CutWoodJobGiver(this);
        Jobs = new JobSystem(new IJobGiver[] { HaulJobGiver, deconstructJobGiver, constructionJobGiver, cutWoodJobGiver });
    }

    public void Initialize(IMapGenerator mapGenerator)
    {
        Speed = 1;
        
        Messaging.Register(UserInterface.Messaging);
        
        var (map, entities) = mapGenerator.GenerateNewMap();
        GameMap = map;
        entities.ForEach(AddEntity);

        Pathfinding = new HexagonPathfinding();
        Pathfinding.BuildMap(GameMap);
        Messaging.Register(Pathfinding.Messaging);
    }

    public CharacterEntity AddCharacter(string label)
    {
        var characterEntity = new CharacterEntity(this, label)
        {
            Position = HexCubeCoord.Zero,
        };

        AddEntity(characterEntity);

        return characterEntity;
    }

    public void Update()
    {
        Messaging.DistributeMessages();
        
        Pathfinding.Update();
        foreach (var entity in _entities.ToList())
        {
            entity.Update();
        }

        _entities.Where(ent => ent.IsRemoved).ToList().ForEach(RemoveEntity);

        UserInterface.Update();
    }

    public ConstructionEntity? StartConstruction(HexCubeCoord position, HexagonDirection rotation, ConstructionDefinition construction)
    {
        var constructionExists = _entities.OfType<ConstructionEntity>().Any(x => x.Position == position);
        var isTerrainPassable = GameMap.CellForPosition(position).Surface.IsPassable;
        ConstructionEntity? constructionEntity = null;
        if (!constructionExists && isTerrainPassable)
        {
            constructionEntity = new ConstructionEntity(position, rotation, construction);
            AddEntity(constructionEntity);
        }

        return constructionEntity;
    }

    public IReadOnlyList<IEntity> EntitiesOn(HexCubeCoord position)
    {
        return _entities.Where(c => c.OccupiedCells.Contains(position)).ToList();
    }

    public BuildingEntity SpawnBuilding(HexCubeCoord position, HexagonDirection rotation, BuildingDefinition buildingDefinition)
    {
        var buildingEntity = new BuildingEntity(position, rotation, buildingDefinition);
        AddEntity(buildingEntity);

        var stuckCharacter = _entities.OfType<CharacterEntity>().FirstOrDefault(c => c.Position == position);
        if (stuckCharacter != null)
        {
            var unstuckCell = stuckCharacter.Position.Neighbors().Where(p => Pathfinding.IsPassable(p))
                .Select(c => GameMap.CellForPosition(c)).FirstOrDefault();
            if (unstuckCell != null)
            {
                stuckCharacter.Position = unstuckCell.Position;
            }
        }

        return buildingEntity;
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

    public void SpawnItem(HexCubeCoord position, ItemDefinition item, int count)
    {
        var existingEntity = _entities.OfType<ItemEntity>()
            .FirstOrDefault(i => i.Position == position && i.Definition == item);
        if (existingEntity == null)
        {
            var itemEntity = new ItemEntity(position, item, count);
            itemEntity.SetHolder(MapItems);
            AddEntity(itemEntity);
        }
        else
        {
            existingEntity.AddCount(count);
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

    public void Designate(HexCubeCoord position, DesignationDefinition designation)
    {
        var buildingsOnPosition = EntitiesOn(position).OfType<BuildingEntity>();
        foreach (var building in buildingsOnPosition)
        {
            building.TryDesignate(designation);            
        }
    }
}