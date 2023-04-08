using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace IsekaiWorld;

public class GameTestInstance
{
    private readonly GameEntity _game;
    private readonly MessagingHub _messageHub;
    private readonly MessagingEndpoint _messaging;

    private readonly Dictionary<String, CharacterTestView> _characterTestViews = new();
    private readonly Dictionary<String, ItemTestView> _itemTestViews = new();
    private readonly Dictionary<String, ConstructionTestView> _constructionTestViews = new();
    private readonly Dictionary<String, BuildingTestView> _buildingTestViews = new();

    public GameTestInstance()
    {
        _game = new GameEntity();
        _game.Initialize(new EmptyMapGenerator());

        _messageHub = new MessagingHub();
        _game.Messaging.ConnectMessageHub(_messageHub);

        _messaging = new MessagingEndpoint(MessageHandler);
        _messageHub.Register(_messaging);
    }

    public HexagonalMapEntity GameMap => _game.GameMap;
    public IReadOnlyList<ConstructionTestView> Constructions => _constructionTestViews.Values.ToList();
    public IReadOnlyList<BuildingTestView> Buildings => _buildingTestViews.Values.ToList();
    public IReadOnlyList<ItemTestView> Items => _itemTestViews.Values.ToList();

    public bool Paused
    {
        get => _game.Paused;
        set => _game.Paused = value;
    }

    public BuildingTestView SpawnBuilding(HexCubeCoord position, HexagonDirection direction,
        BuildingDefinition building)
    {
        var buildingEntity = _game.SpawnBuilding(position, direction, building);
        var buildingTestView = _buildingTestViews.GetOrAdd(buildingEntity.Id.ToString(), id => new BuildingTestView(id));
        _messageHub.DistributeMessages();
        return buildingTestView;
    }

    public CharacterTestView AddCharacter(string name, HexCubeCoord position, bool disableHunger = false)
    {
        var character = _game.AddCharacter(name, disableHunger:disableHunger);
        character.Position = position;

        var testView = new CharacterTestView(character.Id.ToString(), _messaging);
        _characterTestViews[testView.Id] = testView;
        return testView;
    }

    public void Update()
    {
        _game.Update();
        _messageHub.DistributeMessages();
    }

    public IEnumerable<string> CheckForIssues()
    {
        return _game.CheckForIssues();
    }

    public ConstructionTestView StartConstruction(HexCubeCoord position, HexagonDirection direction,
        ConstructionDefinition construction)
    {
        var constructionEntity = _game.StartConstruction(position, direction, construction) ??
                                 throw new Exception("Should start construction");
        return _constructionTestViews.GetOrAdd(constructionEntity.Id.ToString(), id => new ConstructionTestView(id));
    }

    public void UpdateUntil(Func<GameTestStep, bool> check, int maxSteps = 1000, string? because = null)
    {
        var timedOut = UpdateUntilInner(check, maxSteps);

        if (timedOut)
        {
            throw new Exception($"Didn't reach final check before timeout because: {because}");
        }
    }

    private void MessageHandler(IEntityMessage evnt)
    {
        if (evnt is CharacterCreated cc)
        {
            _characterTestViews[cc.EntityId].UpdateFrom(cc);
        }
        else if (evnt is CharacterUpdated cu)
        {
            _characterTestViews[cu.EntityId].UpdateFrom(cu);
        }
        else if (evnt is ItemUpdated iu)
        {
            _itemTestViews.GetOrAdd(iu.EntityId, id => new ItemTestView(id)).UpdateFrom(iu);
        }
        else if (evnt is ItemPickedUp ipu)
        {
            _itemTestViews.Remove(ipu.EntityId);
        }
        else if (evnt is BuildingUpdated bu)
        {
            _buildingTestViews.GetOrAdd(bu.EntityId, id => new BuildingTestView(id)).UpdateFrom(bu);
        }
        else if (evnt is BuildingRemoved br)
        {
            _buildingTestViews.Remove(br.EntityId);
        }
        else if (evnt is ConstructionUpdated coUpd)
        {
            _constructionTestViews.GetOrAdd(coUpd.EntityId, id => new ConstructionTestView(id)).UpdateFrom(coUpd);
        }
        else if (evnt is ConstructionRemoved coRmv)
        {
            _constructionTestViews.Remove(coRmv.EntityId);
        }
    }

    private bool UpdateUntilInner(Func<GameTestStep, bool> check, int maxSteps)
    {
        int steps = 0;
        while (!check(new GameTestStep(this)))
        {
            if (steps >= maxSteps)
            {
                return true;
            }

            steps++;
            Update();

            var issues = _game.CheckForIssues().ToList();
            issues.Should().BeEmpty();
        }

        return false;
    }

    public BuildingTestView SpawnStockpile(HexCubeCoord position)
    {
        return SpawnBuilding(position, HexagonDirection.Left, BuildingDefinitions.StockpileZone);
    }

    public ItemTestView SpawnItem(HexCubeCoord position, ItemDefinition item, int quantity)
    {
        var itemEntity = _game.SpawnItem(position, item, quantity);
        var itemTestView = _itemTestViews.GetOrAdd(itemEntity.EntityId.ToString(), id => new ItemTestView(id));
        _messageHub.DistributeMessages();
        return itemTestView;
    }

    public void Designate(HexCubeCoord position, DesignationDefinition designation)
    {
        _game.Designate(position, designation);
        Update();
    }

    public IReadOnlyList<IEntity> EntitiesOn(HexCubeCoord position)
    {
        return _game.EntitiesOn(position);
    }
}