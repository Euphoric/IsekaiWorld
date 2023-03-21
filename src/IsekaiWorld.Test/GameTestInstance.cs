using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace IsekaiWorld.Test;

public class GameTestInstance
{
    private readonly GameEntity _game;

    private readonly Dictionary<String, CharacterTestView> _characterTestViews = new();
    
    public GameTestInstance()
    {
        _game = new GameEntity();
        _game.Initialize(new EmptyMapGenerator());

        var messaging = new MessagingEndpoint(MessageHandler);
        _game.Messaging.Register(messaging);
    }

    public HexagonalMapEntity GameMap => _game.GameMap;
    public IReadOnlyList<ConstructionEntity> Constructions => _game.Constructions;
    public IReadOnlyList<BuildingEntity> Buildings => _game.Buildings;
    public IReadOnlyList<ItemEntity> Items => _game.Items;

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
        
        var testView = new CharacterTestView(character.Id.ToString());
        _characterTestViews[testView.Id] = testView;
        return testView;
    }

    public void Update()
    {
        _game.Update();
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

    public void UpdateUntil(Func<GameTestStep, bool> check, int maxSteps = 1000, string? title = null)
    {
        var timedOut = UpdateUntilInner(check, maxSteps);

        if (timedOut)
        {
            throw new Exception("Didn't reach final check before timeout.");
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
    }

    private bool UpdateUntilInner(Func<GameTestStep, bool> check, int maxSteps = 1000)
    {
        int steps = 0;
        while (!check(new GameTestStep(_game)))
        {
            if (steps >= maxSteps)
            {
                return true;
            }

            steps++;
            _game.Update();

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