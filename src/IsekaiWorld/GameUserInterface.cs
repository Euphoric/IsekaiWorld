using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

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
            Label = "Character: " + character.Label;
        }

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

        public string Label { get; private set; }

        public bool Update()
        {
            var progress = Mathf.FloorToInt(_construction.ProgressRelative * 100);
            Label = $"Construction: {_construction.BuildingDefinition.Label} Progress: {progress}";

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

        public string Label { get; private set; }

        public bool Update()
        {
            Label = $"Item: {_item.Definition.Label} Count: {_item.Count}";
            return true;
        }
    }

    private readonly GameEntity _game;

    private bool _selectedLabelDirty;
    private ISelection _currentSelection;

    public GameUserInterface(GameEntity game)
    {
        _game = game;
    }

    public IEnumerable<INodeOperation> Update()
    {
        if (_selectedLabelDirty)
        {
            yield return new UpdateSelectedEntityOperation(_currentSelection.Label);
            _selectedLabelDirty = false;
        }

        if (_currentSelection != null)
        {
            if (_currentSelection.Update())
            {
                yield return new UpdateSelectedEntityOperation(_currentSelection.Label);
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
        PlaceItem
    }

    private Tool _currentTool = Tool.Selection;
    private BuildingDefinition _currentBuildingSelection;
    private ItemDefinition _currentItemSelection;

    public void MouseClickOnMap(HexCubeCoord clickPosition)
    {
        switch (_currentTool)
        {
            case Tool.Selection:
                _game.UserInterface.SelectItemOn(clickPosition);
                break;
            case Tool.Construction:
                _game.StartConstruction(clickPosition, _currentBuildingSelection);
                break;
            case Tool.PlaceBuilding:
                _game.SpawnBuilding(clickPosition, _currentBuildingSelection);
                break;
            case Tool.PlaceItem:
                _game.SpawnItem(clickPosition, _currentItemSelection);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SelectionSelected()
    {
        _currentTool = Tool.Selection;
    }

    public void ConstructionSelected(BuildingDefinition buildingDefinition)
    {
        _currentTool = Tool.Construction;
        _currentBuildingSelection = buildingDefinition;
    }

    public void PlaceBuildingSelected(BuildingDefinition buildingDefinition)
    {
        _currentTool = Tool.PlaceBuilding;
        _currentBuildingSelection = buildingDefinition;
    }

    public void PlaceItemSelected(ItemDefinition itemDefinition)
    {
        _currentTool = Tool.PlaceItem;
        _currentItemSelection = itemDefinition;
    }
}

public class UpdateSelectedEntityOperation : INodeOperation
{
    private readonly string _selectionLabelText;

    public UpdateSelectedEntityOperation(string selectionLabelText)
    {
        _selectionLabelText = selectionLabelText;
    }

    public void Execute(GameNode gameNode)
    {
        var selectionLabel = gameNode.GetNode<Label>("/root/GameNode/UserInterface/SelectionLabel");
        selectionLabel.Text = _selectionLabelText;
    }
}