using System;
using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld;

public class GameUserInterface
{
    private interface ISelection
    {
        string Label { get; }
        bool Update();
    }

    private class CharacterSelection : ISelection
    {
        public CharacterSelection(CharacterEntity character)
        {
            Id = character.Id.ToString();
            Label = "Character: " + character.Label;
        }

        public String Id { get; }

        public string Label { get; }

        public bool Update()
        {
            return false;
        }
    }

    private class BuildingSelection : ISelection
    {
        public BuildingSelection(BuildingEntity building)
        {
            Label = "Building: " + building.Label;
        }

        public string Label { get; }

        public bool Update()
        {
            return false;
        }
    }

    private class ConstructionSelection : ISelection
    {
        private readonly ConstructionEntity _construction;

        public ConstructionSelection(ConstructionEntity construction)
        {
            _construction = construction;
        }

        public string Label { get; private set; } = "";

        public bool Update()
        {
            var progress = Godot.Mathf.FloorToInt(_construction.ProgressRelative * 100);
            Label = $"Construction: {_construction.Definition.Label} Progress: {progress}";

            return true;
        }
    }

    private class ItemSelection : ISelection
    {
        private readonly ItemEntity _item;

        public ItemSelection(ItemEntity item)
        {
            _item = item;
        }

        public string Label { get; private set; } = "";

        public bool Update()
        {
            Label = $"Item: {_item.Definition.Label} Count: {_item.Count}";
            return true;
        }
    }

    [Obsolete] private readonly GameEntity _game;

    private bool _selectedLabelDirty;

    private ISelection? _currentSelection;

    public MessagingEndpoint Messaging { get; }

    public GameUserInterface(GameEntity game)
    {
        _game = game;
        Messaging = new MessagingEndpoint(HandleMessage);
    }

    private Dictionary<String, int> _selectionEntities = new();

    private void HandleMessage(IEntityMessage mssg)
    {
        switch (mssg)
        {
            case DesignationToolSelect msg:
                DesignateTool(msg.Designation);
                break;
        }
    }

    public HexagonDirection ConstructionRotation { get; set; }

    public void Update()
    {
        if (_selectedLabelDirty)
        {
            Messaging.Broadcast(new SelectionChanged(_currentSelection?.Label));
            _selectedLabelDirty = false;
        }

        if (_currentSelection != null)
        {
            if (_currentSelection.Update())
            {
                Messaging.Broadcast(new SelectionChanged(_currentSelection?.Label));
            }
        }
    }

    private void SelectItemOn(HexCubeCoord position)
    {
        var selectedEntity = _game.EntitiesOn(position).FirstOrDefault();

        if (selectedEntity != null)
        {
            _selectedLabelDirty = true;
            if (selectedEntity is CharacterEntity selectedCharacter)
            {
                _currentSelection = new CharacterSelection(selectedCharacter);
            }
            else if (selectedEntity is BuildingEntity selectedBuilding)
            {
                _currentSelection = new BuildingSelection(selectedBuilding);
            }
            else if (selectedEntity is ConstructionEntity selectedConstruction)
            {
                _currentSelection = new ConstructionSelection(selectedConstruction);
            }
            else if (selectedEntity is ItemEntity itemEntity)
            {
                _currentSelection = new ItemSelection(itemEntity);
            }
            else
            {
                throw new Exception("Unknown entity type: " + selectedEntity.GetType());
            }
        }
    }

    enum Tool
    {
        Selection,
        Construction,
        PlaceBuilding,
        PlaceItem,
        Designate
    }

    private Tool _currentTool = Tool.Selection;
    private ConstructionDefinition _currentBuildingSelection = null!;
    private ItemDefinition _currentItemSelection = null!;
    private DesignationDefinition _currentDesignation = null!;

    public void MouseClickOnMap(HexCubeCoord clickPosition)
    {
        switch (_currentTool)
        {
            case Tool.Selection:
                SelectItemOn(clickPosition);
                break;
            case Tool.Construction:
                _game.StartConstruction(clickPosition, ConstructionRotation, _currentBuildingSelection);
                break;
            case Tool.PlaceBuilding:
                // TODO: Fix
                // _game.SpawnBuilding(clickPosition, _currentBuildingSelection);
                break;
            case Tool.PlaceItem:
                _game.SpawnItem(clickPosition, _currentItemSelection, 1);
                break;
            case Tool.Designate:
                _game.Designate(clickPosition, _currentDesignation);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SelectionSelected()
    {
        _currentTool = Tool.Selection;
    }

    public void ConstructionSelected(ConstructionDefinition definition)
    {
        _currentTool = Tool.Construction;
        _currentBuildingSelection = definition;
    }

    // public void PlaceBuildingSelected(BuildingDefinition buildingDefinition)
    // {
    //     _currentTool = Tool.PlaceBuilding;
    //     _currentBuildingSelection = buildingDefinition;
    // }

    public void PlaceItemSelected(ItemDefinition itemDefinition)
    {
        _currentTool = Tool.PlaceItem;
        _currentItemSelection = itemDefinition;
    }

    private void DesignateTool(DesignationDefinition designation)
    {
        _currentTool = Tool.Designate;
        _currentDesignation = designation;
    }

    public void SetSpeed(int speed)
    {
        _game.Speed = speed;
    }

    public void TogglePause()
    {
        _game.Paused = !_game.Paused;
    }

    public void SetCharacterHealth()
    {
        if (_currentSelection is CharacterSelection cs)
        {
            Messaging.Broadcast(new SetCharacterHunger(cs.Id, 0.31));
        }
    }
}

record SelectionChanged(string? SelectionLabel) : IEntityMessage;

record DesignationToolSelect(DesignationDefinition Designation) : IEntityMessage;