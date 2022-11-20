using System;
using Godot;
using GodotArray = Godot.Collections.Array;

public partial class UserInterface : CanvasLayer
{
    public MessagingEndpoint Messaging { get; }

    private GameUserInterface _gameUserInterface = null!;

    public UserInterface()
    {
        Messaging = new MessagingEndpoint(MessageHandler);
    }

    [Obsolete("Should not be needed")]
    public void Initialize(GameEntity gameEntity)
    {
        _gameUserInterface = gameEntity.UserInterface;
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

    private void MessageHandler(IEntityMessage message)
    {
        switch (message)
        {
            case SelectionChanged selectedEntityChanged:
                OnSelectedEntityChanged(selectedEntityChanged);
                break;
            case TpsChanged tpsChanged:
                OnTpsChanged(tpsChanged);
                break;
            case SpeedChanged speedChanged:
                OnSpeedChanged(speedChanged);
                break;
        }
    }

    public override void _Input(InputEvent evnt)
    {
        if (evnt.IsAction("speed_1"))
        {
            _gameUserInterface.SetSpeed(1);
        }
        else if (evnt.IsAction("speed_2"))
        {
            _gameUserInterface.SetSpeed(2);
        }
        else if (evnt.IsAction("speed_3"))
        {
            _gameUserInterface.SetSpeed(3);
        }
        else if (evnt.IsAction("speed_4"))
        {
            _gameUserInterface.SetSpeed(4);
        }
        else if (evnt.IsAction("speed_5"))
        {
            _gameUserInterface.SetSpeed(5);
        }

        base._Input(evnt);
    }

    private void OnSelectedEntityChanged(SelectionChanged message)
    {
        var selectionLabel = GetNode<Label>("SelectionLabel");
        selectionLabel.Text = message.SelectionLabel;
    }

    private void OnTpsChanged(TpsChanged tpsChanged)
    {
        var tpsLabel = GetNode<Label>("Container/TpsLabel");
        tpsLabel.Text = tpsChanged.Tps.ToString("F0");
    }

    private void OnSpeedChanged(SpeedChanged speedChanged)
    {
        var speedLabel = GetNode<Label>("Container/SpeedLabel");
        speedLabel.Text = speedChanged.Speed + "x";
    }

    // ReSharper disable once UnusedMember.Global
    public void _on_SelectionButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;

        ToolLabel.Text = "Selection";

        _gameUserInterface.SelectionSelected();
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
            _gameUserInterface.ConstructionSelected(definition);
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
        _gameUserInterface.PlaceItemSelected(itemDefinition);
        ToolLabel.Text = "Place item";
    }

    public void _on_rotation_selected(long index)
    {
        _gameUserInterface.ConstructionRotation = (HexagonDirection)index;
    }

    private void _on_CutWoodButton_pressed()
    {
        _gameUserInterface.DesignateCutWoodSelected();
        ToolLabel.Text = "Cut tree";
    }

    private void _on_DeconstructButton_pressed()
    {
        _gameUserInterface.DesignateDeconstructSelected();
        ToolLabel.Text = "Deconstruct";
    }
}