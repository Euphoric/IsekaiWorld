using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace IsekaiWorld.Test;

public class GameTestInstance
{
    private readonly GameEntity _game;
    private readonly MessagingHub _messageHub;
    private readonly MessagingEndpoint _messaging;

    private readonly Dictionary<String, CharacterTestView> _characterTestViews = new();
    private readonly Dictionary<String, ItemTestView> _itemTestViews = new();

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
    public IReadOnlyList<ConstructionEntity> Constructions => _game.Constructions;
    public IReadOnlyList<BuildingEntity> Buildings => _game.Buildings;
    public IReadOnlyList<ItemTestView> Items => _itemTestViews.Values.ToList();

    public bool Paused
    {
        get => _game.Paused;
        set => _game.Paused = value;
    }

    public BuildingEntity SpawnBuilding(HexCubeCoord position, HexagonDirection direction, BuildingDefinition building)
    {
        return _game.SpawnBuilding(position, direction, building);
    }

    public CharacterTestView AddCharacter(string name, HexCubeCoord position)
    {
        var character = _game.AddCharacter(name);
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

    public ConstructionEntity? StartConstruction(HexCubeCoord position, HexagonDirection direction,
        ConstructionDefinition construction)
    {
        return _game.StartConstruction(position, direction, construction);
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

        if (evnt is CharacterUpdated cu)
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
    }

    private bool UpdateUntilInner(Func<GameTestStep, bool> check, int maxSteps)
    {
        int steps = 0;
        while (!check(new GameTestStep(_game)))
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

    public BuildingEntity SpawnStockpile(HexCubeCoord position)
    {
        var stockpile = new BuildingEntity(position, HexagonDirection.Left, BuildingDefinitions.StockpileZone);
        _game.AddEntity(stockpile);
        return stockpile;
    }

    public void SpawnItem(HexCubeCoord position, ItemDefinition item, int quantity)
    {
        _game.SpawnItem(position, item, quantity);
    }

    public void Designate(HexCubeCoord position, DesignationDefinition designation)
    {
        _game.Designate(position, designation);
    }

    public IReadOnlyList<IEntity> EntitiesOn(HexCubeCoord position)
    {
        return _game.EntitiesOn(position);
    }
}