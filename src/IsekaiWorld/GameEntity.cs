using System;
using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class GameEntity
{
    public HexagonalMapEntity GameMap { get; private set; } = null!;
    public HexagonPathfinding Pathfinding { get; private set; } = null!;
    public SequenceCompositeActivityPlanner Jobs { get; }
    public MessagingHub MessagingHub { get; }
    public MessagingEndpoint Messaging { get; }

    public IReadOnlyList<ConstructionEntity> Constructions => _entities.OfType<ConstructionEntity>().ToList();
    public IReadOnlyList<BuildingEntity> Buildings => _entities.OfType<BuildingEntity>().ToList();

    /// <summary>
    /// All items in game. Including the ones being carried by characters.
    /// </summary>
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

    /// <summary>
    /// Items laying on the map.
    /// </summary>
    public IEnumerable<ItemEntity> MapItems => Items.Where(it => it.IsMapItem);

    public MapItems MapItemsHolder { get; } = new();


    public GameEntity()
    {
        MessagingHub = new MessagingHub();
        Messaging = new MessagingEndpoint(HandleMessage);
        var eatingPlanner = new EatFoodActivityPlanner(this);
        var haulingPlanner = new HaulActivityPlanner(this);
        var decustructPlanner = new DeconstructActivityPlanner(this);
        var constructionPlanner = new ConstructionActivityPlanner(this);
        var craftingPlanner = new CraftingPlanner(this);
        var cutWoodPlanner = new CutWoodActivityPlanner(this);
        var haulPlanner = new GatherActivityPlanner(this);
        Jobs = new SequenceCompositeActivityPlanner(new IActivityPlanner[]
        {
            eatingPlanner, decustructPlanner, constructionPlanner, craftingPlanner, cutWoodPlanner, haulPlanner,
            haulingPlanner
        });
    }

    private void HandleMessage(IEntityMessage mssg)
    {
        switch (mssg)
        {
            case SetSpeed msg:
                Speed = msg.Speed;
                break;
            case TogglePause:
                Paused = !Paused;
                break;
            case StartConstruction msg:
                StartConstruction(msg.Position, msg.Rotation, msg.Definition);
                break;
            case SpawnItem msg:
                SpawnItem(msg.Position, msg.Definition, msg.Count);
                break;
            case Designate msg:
                Designate(msg.Position, msg.Designation);
                break;
            case SpawnBuilding msg:
                SpawnBuilding(msg.Position, msg.Rotation, msg.Building);
                break;
            case AddCraftingBill msg:
                AddCraftingBill(msg.CraftingDefinition);
                break;
        }
    }


    public void Initialize(IMapGenerator mapGenerator)
    {
        MessagingHub.Register(Messaging);

        Speed = 1;

        GameMap = new HexagonalMapEntity(mapGenerator.MapSize);
        mapGenerator.GenerateMap(this);

        Pathfinding = new HexagonPathfinding();
        Pathfinding.BuildMap(GameMap);
        MessagingHub.Register(Pathfinding.Messaging);

        // Probably shouldn't be here. Find better place for initial surface update.
        Messaging.Broadcast(new SurfaceChanged(GameMap.Cells));
    }

    public CharacterEntity AddCharacter(string label, bool disableHunger = false)
    {
        var characterEntity = new CharacterEntity(this, label)
        {
            Position = HexCubeCoord.Zero,
            DisableHunger = disableHunger
        };

        AddEntity(characterEntity);

        return characterEntity;
    }

    public void Update()
    {
        MessagingHub.DistributeMessages();

        Pathfinding.Update();

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int i = 0; i < _entities.Count; i++)
        {
            _entities[i].Update();
        }

        _entities.Where(ent => ent.IsRemoved).ToList()
            .ForEach(entity =>
            {
                _entities.Remove(entity);
                MessagingHub.Unregister(entity.Messaging);
            });
    }

    public ConstructionEntity? StartConstruction(HexCubeCoord position, HexagonDirection rotation,
        ConstructionDefinition construction)
    {
        var constructionExists = _entities.OfType<ConstructionEntity>().Any(x => x.Position == position);
        var isTerrainPassable = GameMap.CellForPosition(position).Surface.IsPassable;
        var buildingExists = _entities.OfType<BuildingEntity>().Any(x => x.Position == position);

        var canPlaceSurface = GameMap.CellForPosition(position).Surface != construction.PlaceFloor;

        ConstructionEntity? constructionEntity = null;
        if (!constructionExists && !buildingExists && isTerrainPassable && canPlaceSurface)
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

    public BuildingEntity SpawnBuilding(HexCubeCoord position, HexagonDirection rotation,
        BuildingDefinition buildingDefinition)
    {
        var buildingEntity = new BuildingEntity(position, rotation, buildingDefinition);
        AddEntity(buildingEntity);

        return buildingEntity;
    }

    public void SetFloor(HexCubeCoord position, SurfaceDefinition surface)
    {
        GameMap.SetCellSurface(position, surface);
        Messaging.Broadcast(new SurfaceChanged(GameMap.Cells));
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

    public ItemEntity SpawnItem(HexCubeCoord position, ItemDefinition item, int count)
    {
        var existingEntity = _entities.OfType<ItemEntity>()
            .FirstOrDefault(i => i.Position == position && i.Definition == item);
        if (existingEntity == null)
        {
            var itemEntity = new ItemEntity(this, position, item, count);
            itemEntity.SetHolder(MapItemsHolder);
            AddEntity(itemEntity);
            itemEntity.Initialize();
            return itemEntity;
        }
        else
        {
            existingEntity.AddCount(count);
            return existingEntity;
        }
    }

    public void AddEntity(IEntity entity)
    {
        MessagingHub.Register(entity.Messaging);
        _entities.Add(entity);
        entity.Initialize();
    }

    public void Designate(HexCubeCoord position, DesignationDefinition designation)
    {
        var buildingsOnPosition = EntitiesOn(position).OfType<BuildingEntity>();
        foreach (var building in buildingsOnPosition)
        {
            building.TryDesignate(designation);
        }
    }

    public CraftingDefinition? BillToCraft { get; set; }

    public void AddCraftingBill(CraftingDefinition billToCraft)
    {
        BillToCraft = billToCraft;
    }
}