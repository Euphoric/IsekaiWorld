using System;
using System.Collections.Generic;
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

        public string Label { get;  }
        
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

        public string Label { get;  }
        
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
        var selectedCharacter = _game.CharacterOn(position);
        if (selectedCharacter != null)
        {
            _selectedLabelDirty = true;
            _currentSelection = new CharacterSelection(selectedCharacter);
        }

        var selectedBuilding = _game.BuildingOn(position);
        if (selectedBuilding != null)
        {
            _selectedLabelDirty = true;
            _currentSelection = new BuildingSelection(selectedBuilding);
        }

        var selectedConstruction = _game.ConstructionOn(position);
        if (selectedConstruction != null)
        {
            _selectedLabelDirty = true;
            _currentSelection = new ConstructionSelection(selectedConstruction);
        }
    }

    enum Tool
    {
        Selection,
        Construction
    }

    private Tool _currentTool = Tool.Selection;
    
    public void MouseClickOnMap(HexCubeCoord clickPosition)
    {
        switch (_currentTool)
        {
            case Tool.Selection:
                _game.UserInterface.SelectItemOn(clickPosition);
                break;
            case Tool.Construction:
                _game.StartConstruction(clickPosition);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SelectionToggled()
    {
        _currentTool = Tool.Selection;
    }

    public void ContructionToggled()
    {
        _currentTool = Tool.Construction;
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