using System;
using System.Collections.Generic;
using Godot;

public class GameUserInterface
{
    private readonly GameEntity _game;

    private bool _selectedCharacterDirty;
    private CharacterEntity _selectedCharacter;

    public GameUserInterface(GameEntity game)
    {
        _game = game;
    }
    
    public IEnumerable<INodeOperation> Update()
    {
        if (_selectedCharacterDirty)
        {
            yield return new UpdateSelectedEntityOperation("Character: " + _selectedCharacter.Label);
            _selectedCharacterDirty = false;
        }
    }
    
    public void SelectItemOn(HexCubeCoord position)
    {
        var selectedCharacter = _game.EntityOn(position);
        if (selectedCharacter != null && selectedCharacter != _selectedCharacter)
        {
            _selectedCharacterDirty = true;
            _selectedCharacter = selectedCharacter;
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
    
    public void Execute(HexagonalMap map)
    {
        var selectionLabel = map.GetNode<Label>("/root/Game/UI/SelectionLabel");
        selectionLabel.Text = _selectionLabelText;
    }
}