using Godot;
using GodotArray = Godot.Collections.Array;

public class UserInterface : CanvasLayer
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

        foreach (var definition in BuildingDefinitions.Definitions)
        {
            var button = new Button
            {
                Text = definition.Label,
            };
            string buildingId = definition.Id;
            button.Connect("pressed", this, nameof(_on_ConstructionSelectionButton_pressed), new GodotArray { buildingId }); 
            ConstructionContainer.AddChild(button);            
        }
        
        foreach (var definition in ItemDefinitions.Definitions)
        {
            var button = new Button
            {
                Text = definition.Label,
            };
            string itemId = definition.Id;
            button.Connect("pressed", this, nameof(_on_PlaceItemSelectionButton_pressed), new GodotArray { itemId }); 
            PlaceItemContainer.AddChild(button);            
        }

        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;
    }

    public Label ToolLabel => GetNode<Label>("ToolLabel");
    public Container ConstructionContainer => GetNode<Container>("ConstructionContainer");
    public CheckButton PlaceDirectlyButton => ConstructionContainer.GetNode<CheckButton>("PlaceDirectlyButton");
    
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
        ConstructionContainer.Visible = !ConstructionContainer.Visible;
    }

    public void _on_ConstructionSelectionButton_pressed(string buildingDefinitionId)
    {
        var buildingDefinition = BuildingDefinitions.GetById(buildingDefinitionId);
        if (!PlaceDirectlyButton.Pressed)
        {
            _game.UserInterface.ConstructionSelected(buildingDefinition);
            ToolLabel.Text = "Construction: " + buildingDefinition.Label;
        }
        else
        {
            _game.UserInterface.PlaceBuildingSelected(buildingDefinition);
            ToolLabel.Text = "Place building: " + buildingDefinition.Label;
        }
    }
    
    // ReSharper disable once UnusedMember.Global
    public void _on_PlaceItemButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = !PlaceItemContainer.Visible;
    }
    
    public void _on_PlaceItemSelectionButton_pressed(string itemDefinitionId)
    {
        var itemDefinition = ItemDefinitions.GetById(itemDefinitionId);
        _game.UserInterface.PlaceItemSelected(itemDefinition);
        ToolLabel.Text = "Place item";
    }
}
