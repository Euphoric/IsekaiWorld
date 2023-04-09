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
        public CharacterSelection(CharacterEntityItem character)
        {
            Id = character.Id;
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
        public BuildingSelection(BuildingEntityItem building)
        {
            Label = "Building: " + building.Definition.Label;
        }

        public string Label { get; }

        public bool Update()
        {
            return false;
        }
    }

    private class ConstructionSelection : ISelection
    {
        private readonly ConstructionEntityItem _construction;

        public ConstructionSelection(ConstructionEntityItem construction)
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
        private readonly ItemEntityItem _item;

        public ItemSelection(ItemEntityItem item)
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

    private abstract class EntityItem
    {
        public string Id { get; }

        public EntityItem(String id)
        {
            Id = id;
        }

        public void Update(HexCubeCoord position)
        {
            Position = position;
        }

        public HexCubeCoord Position { get; set; }
    }

    private class ConstructionEntityItem : EntityItem
    {
        public ConstructionEntityItem(string id) : base(id)
        {
        }

        public void Update(ConstructionUpdated msg)
        {
            base.Update(msg.Position);
            Definition = msg.Definition;
            ProgressRelative = msg.ProgressRelative;
        }

        public ConstructionDefinition Definition { get; set; } = null!;

        public float ProgressRelative { get; set; }
    }

    private class BuildingEntityItem : EntityItem
    {
        public BuildingEntityItem(string id) : base(id)
        {
        }

        public void Update(BuildingUpdated msg)
        {
            base.Update(msg.Position);
            Definition = msg.Definition;
        }

        public BuildingDefinition Definition { get; set; } = null!;
    }

    private class CharacterEntityItem : EntityItem
    {
        public CharacterEntityItem(string id) : base(id)
        {
        }

        public void Update(CharacterUpdated msg)
        {
            base.Update(msg.Position);
            Label = msg.Label;
        }

        public string Label { get; set; } = null!;
    }

    private class ItemEntityItem : EntityItem
    {
        public ItemEntityItem(string id) : base(id)
        {
        }

        public void Update(ItemUpdated msg)
        {
            base.Update(msg.Position);
            Definition = msg.Definition;
            Count = msg.Count;
        }

        public int Count { get; set; }

        public ItemDefinition Definition { get; set; } = null!;
    }
    
    private readonly Dictionary<String, EntityItem> _selectionEntities = new();

    private void UpdateSelectionEntity<TMsg, TSelection>(TMsg msg, Func<TMsg, String> id, Func<String, TSelection> newFunc, Action<TMsg, TSelection> update)
        where TSelection : EntityItem
    {
        var e = _selectionEntities.GetOrAdd(id(msg), newFunc);
        update(msg, (TSelection)e);
    }
    
    private void HandleMessage(IEntityMessage mssg)
    {
        switch (mssg)
        {
            case DesignationToolSelect msg:
                DesignateTool(msg.Designation);
                break;
            case BuildingUpdated msg:
                UpdateSelectionEntity(msg, m=>m.EntityId,
                    id => new BuildingEntityItem(id),
                    (m, s) => s.Update(m));
                break;
            case BuildingRemoved msg:
                _selectionEntities.Remove(msg.EntityId);
                break;
            case CharacterUpdated msg:
                UpdateSelectionEntity(msg, m=>m.EntityId,
                    id => new CharacterEntityItem(id),
                    (m, s) => s.Update(m));
                break;
            case ConstructionUpdated msg:
                UpdateSelectionEntity(msg, m=>m.EntityId,
                    id => new ConstructionEntityItem(id),
                    (m, s) => s.Update(m));
                break;
            case ConstructionRemoved msg:
                _selectionEntities.Remove(msg.EntityId);
                break;
            case ItemUpdated msg:
                UpdateSelectionEntity(msg, m=>m.EntityId,
                    id => new ItemEntityItem(id),
                    (m, s) => s.Update(m));
                break;
            case ItemPickedUp msg:
                _selectionEntities.Remove(msg.EntityId);
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
        var selectedEntity = _selectionEntities.Values.FirstOrDefault(x => x.Position == position);

        if (selectedEntity != null)
        {
            _selectedLabelDirty = true;
            if (selectedEntity is CharacterEntityItem selectedCharacter)
            {
                _currentSelection = new CharacterSelection(selectedCharacter);
            }
            else if (selectedEntity is BuildingEntityItem selectedBuilding)
            {
                _currentSelection = new BuildingSelection(selectedBuilding);
            }
            else if (selectedEntity is ConstructionEntityItem selectedConstruction)
            {
                _currentSelection = new ConstructionSelection(selectedConstruction);
            }
            else if (selectedEntity is ItemEntityItem itemEntity)
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