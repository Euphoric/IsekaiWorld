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

        ConstructionContainer.Visible = false;
    }

    public Label ToolLabel => GetNode<Label>("ToolLabel");
    public Container ConstructionContainer => GetNode<Container>("ConstructionContainer");
    
    // ReSharper disable once UnusedMember.Global
    public void _on_SelectionButton_pressed()
    {
        ToolLabel.Text = "Selection";
        
        _game.UserInterface.SelectionToggled();
    }
    
    // ReSharper disable once UnusedMember.Global
    public void _on_ConstructionButton_pressed()
    {
        ConstructionContainer.Visible = !ConstructionContainer.Visible;
    }

    public void _on_ConstructionSelectionButton_pressed(string buildingDefinitionId)
    {
        var buildingDefinition = BuildingDefinitions.GetById(buildingDefinitionId);
        _game.UserInterface.ConstructionToggled(buildingDefinition);
        
        ToolLabel.Text = "Construction: " + buildingDefinition.Label;
    }
}
