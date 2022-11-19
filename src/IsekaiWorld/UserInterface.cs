using System;
using Godot;
using GodotArray = Godot.Collections.Array;

public partial class UserInterface : CanvasLayer
{
    private GameEntity _game;

    public override void _EnterTree()
    {
        var gameNode = GetNode<GameNode>("/root/GameNode");
        _game = gameNode.GameEntity;

        base._EnterTree();
    }

    public override void _Ready()
    {
        base._Ready();

        foreach (var definition in ConstructionDefinitions.Definitions)
        {
            var button = new Button
            {
                Text = definition.Label,
            };
            button.Pressed += () => _on_ConstructionSelectionButton_pressed(definition.Id); 
            ConstructionContainer.AddChild(button);            
        }
        
        foreach (var definition in ItemDefinitions.Definitions)
        {
            var button = new Button
            {
                Text = definition.Label,
            };
            button.Pressed += () => _on_PlaceItemSelectionButton_pressed(definition.Id);
            PlaceItemContainer.AddChild(button);            
        }

        foreach (HexagonDirection rotation in Enum.GetValues(typeof(HexagonDirection)))
        {
            RotationOptionButton.AddItem(rotation.ToString(), (int)rotation);
        }
        RotationOptionButton.Selected = 0;
        RotationOptionButton.ItemSelected += _on_rotation_selected;

        {
            var designationButton = GetNode<Button>("BottomMenu/DesignationButton");
            designationButton.Pressed += _on_DesignationButton_pressed;

            {
                var cutWoodButton = new Button();
                cutWoodButton.Text = "Cut wood";
                cutWoodButton.Pressed += _on_CutWoodButton_pressed;
                DesignationContainer.AddChild(cutWoodButton);
            }
            
            {
                var deconstructButton = new Button();
                deconstructButton.Text = "Deconstruct";
                deconstructButton.Pressed += _on_DeconstructButton_pressed;
                DesignationContainer.AddChild(deconstructButton);
            }
        }


        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;
        DesignationContainer.Visible = false;
    }

    public Label ToolLabel => GetNode<Label>("ToolLabel");
    public Container ConstructionContainer => GetNode<Container>("ConstructionContainer");
    public CheckButton PlaceDirectlyButton => ConstructionContainer.GetNode<CheckButton>("PlaceDirectlyButton");
    public OptionButton RotationOptionButton => ConstructionContainer.GetNode<OptionButton>("RotationOptionButton");
    public Container DesignationContainer => GetNode<Container>("DesignationContainer");
    
    public Container PlaceItemContainer => GetNode<Container>("PlaceItemContainer");
    
    // ReSharper disable once UnusedMember.Global
    public void _on_SelectionButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;

        ToolLabel.Text = "Selection";
        
        _game.UserInterface.SelectionSelected();
    }
    
    // ReSharper disable once UnusedMember.Global
    public void _on_ConstructionButton_pressed()
    {
        PlaceItemContainer.Visible = false;
        DesignationContainer.Visible = false;
        ConstructionContainer.Visible = !ConstructionContainer.Visible;
    }
    
    // ReSharper disable once UnusedMember.Global
    public void _on_PlaceItemButton_pressed()
    {
        ConstructionContainer.Visible = false;
        DesignationContainer.Visible = false;
        PlaceItemContainer.Visible = !PlaceItemContainer.Visible;
    }
    
    public void _on_DesignationButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;
        DesignationContainer.Visible = !DesignationContainer.Visible;
    }

    public void _on_ConstructionSelectionButton_pressed(string constructionDefinitionId)
    {
        var definition = ConstructionDefinitions.GetById(constructionDefinitionId);
        if (!PlaceDirectlyButton.ButtonPressed)
        {
            _game.UserInterface.ConstructionSelected(definition);
            ToolLabel.Text = "Construction: " + definition.Label;
        }
        else
        {
            // TODO: Fix
            // _game.UserInterface.PlaceBuildingSelected(definition);
            // ToolLabel.Text = "Place building: " + definition.Label;
        }
    }

    public void _on_PlaceItemSelectionButton_pressed(string itemDefinitionId)
    {
        var itemDefinition = ItemDefinitions.GetById(itemDefinitionId);
        _game.UserInterface.PlaceItemSelected(itemDefinition);
        ToolLabel.Text = "Place item";
    }

    public void _on_rotation_selected(long index)
    {
        _game.UserInterface.ConstructionRotation = (HexagonDirection)index;
    }

    private void _on_CutWoodButton_pressed()
    {
        _game.UserInterface.DesignateCutWoodSelected();
        ToolLabel.Text = "Cut tree";
    }
    
    private void _on_DeconstructButton_pressed()
    {
        _game.UserInterface.DesignateDeconstructSelected();
        ToolLabel.Text = "Deconstruct";
    }
}
