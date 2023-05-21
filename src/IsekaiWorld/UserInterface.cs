using System;
using System.Linq;
using Godot;

namespace IsekaiWorld;

public partial class UserInterface : CanvasLayer
{
    public MessagingEndpoint Messaging { get; }

    private GameUserInterface _gameUserInterface = null!;

    public UserInterface()
    {
        Messaging = new MessagingEndpoint(MessageHandler);
    }

    public void Initialize(GameUserInterface gameUserInterface)
    {
        _gameUserInterface = gameUserInterface;
        _gameUserInterface.OnCurrentSelectionChanged += GameUserInterfaceOnOnCurrentSelectionChanged;
    }

    private void GameUserInterfaceOnOnCurrentSelectionChanged(GameUserInterface.EntitySelection? selection)
    {
        var selectionLabel = GetNode<Label>("BottomMenuArea/SelectionLabel");
        selectionLabel.Text = selection?.TextLabel;
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
            button.Pressed += () => _on_ConstructionSelectionButton_pressed(definition);
            ConstructionContainer.AddChild(button);
        }

        foreach (var definition in ItemDefinitions.Definitions)
        {
            var button = new Button
            {
                Text = definition.Label,
            };
            button.Pressed += () => _on_PlaceItemSelectionButton_pressed(definition);
            PlaceItemContainer.AddChild(button);
        }

        foreach (var definition in BuildingDefinitions.Definitions)
        {
            var button = new Button
            {
                Text = definition.Label,
            };
            button.Pressed += () => _on_BuildingSelectionButton_pressed(definition);
            BuildingContainer.AddChild(button);
        }

        foreach (HexagonDirection rotation in Enum.GetValues(typeof(HexagonDirection)))
        {
            RotationOptionButton.AddItem(rotation.ToString(), (int)rotation);
        }

        RotationOptionButton.Selected = 0;
        RotationOptionButton.ItemSelected += _on_rotation_selected;

        SetUpDesignations();
        SetUpCrafting();
        
        {
            var debugButton = GetNode<Button>("BottomMenuArea/BottomMenu/DebugButton");
            debugButton.Pressed += _on_DebugButton_pressed;

            var setHungerButton = new Button();
            setHungerButton.Text = "Set character hunger to 31/100";
            setHungerButton.Pressed += () => _gameUserInterface.SetCharacterHealth();
            DebugContainer.AddChild(setHungerButton);
        }

        var selectionOptionButton = GetNode<OptionButton>("TopMenuArea/SelectionOptionButton");
        
        foreach (SelectionOptions selectionOption in Enum.GetValues(typeof(SelectionOptions)))
        {
            selectionOptionButton.AddItem(selectionOption.ToString(), (int)selectionOption);
        }
        selectionOptionButton.Selected = 0;
        selectionOptionButton.ItemSelected += _on_selectionOption;
            
        ConstructionContainer.Visible = false;
        BuildingContainer.Visible = false;
        PlaceItemContainer.Visible = false;
        DesignationContainer.Visible = false;
        DebugContainer.Visible = false;
        CraftingContainer.Visible = false;
    }

    private void SetUpDesignations()
    {
        var designationButton = GetNode<Button>("BottomMenuArea/BottomMenu/DesignationButton");
        designationButton.Pressed += _on_DesignationButton_pressed;

        void OnDesignationToolButtonPressed(DesignationDefinition designation)
        {
            _gameUserInterface.DesignateTool(designation);
            ToolLabel.Text = designation.Title;
        }

        foreach (var designation in DesignationDefinitions.All)
        {
            var button = new Button();
            button.Text = designation.Title;
            button.Pressed += () => OnDesignationToolButtonPressed(designation);
            DesignationContainer.AddChild(button);
        }
    }
    
    private void SetUpCrafting()
    {
        var craftingButton = GetNode<Button>("BottomMenuArea/BottomMenu/CraftingButton");
        craftingButton.Pressed += _on_CraftingButton_pressed;

        var craftableItemsList = CraftingContainer.GetNode<ItemList>("CraftableItemsList");

        foreach (var designation in CraftingDefinitions.Definitions)
        {
            var idx = craftableItemsList.AddItem(designation.Title);
            craftableItemsList.SetItemMetadata(idx, designation.Id);
        }

        var craftItemBtn = CraftingContainer.GetNode<Button>("CraftItem");
        craftItemBtn.Pressed += OnCraftItemBtnPressed;
    }
    
    void OnCraftItemBtnPressed()
    {
        var craftableItemsList = CraftingContainer.GetNode<ItemList>("CraftableItemsList");
        
        var selectedIndex = craftableItemsList.GetSelectedItems().Cast<int?>().SingleOrDefault();
        if (selectedIndex == null) 
            return;
        
        var selectedCraftingId = craftableItemsList.GetItemMetadata(selectedIndex.Value).AsString();
        var craftingDef = CraftingDefinitions.GetById(selectedCraftingId);

        _gameUserInterface.AddCraftingBill(craftingDef);
    }

    public Label ToolLabel => GetNode<Label>("BottomMenuArea/ToolLabel");
    public Container ConstructionContainer => GetNode<Container>("TopMenuArea/Menus/ConstructionContainer");
    public Container BuildingContainer => GetNode<Container>("TopMenuArea/Menus/BuildingContainer");
    public OptionButton RotationOptionButton => GetNode<OptionButton>("TopMenuArea/RotationOptionButton");
    public Container DesignationContainer => GetNode<Container>("TopMenuArea/Menus/DesignationContainer");
    public Container PlaceItemContainer => GetNode<Container>("TopMenuArea/Menus/PlaceItemContainer");
    public Container CraftingContainer => GetNode<Container>("TopMenuArea/Menus/CraftingContainer");
    public Container DebugContainer => GetNode<Container>("TopMenuArea/Menus/DebugContainer");

    private void MessageHandler(IEntityMessage message)
    {
        switch (message)
        {
            case TpsChanged tpsChanged:
                OnTpsChanged(tpsChanged);
                break;
            case SpeedChanged speedChanged:
                OnSpeedChanged(speedChanged);
                break;
        }
    }

    public override void _Process(double delta)
    {
        _gameUserInterface.Update();

        var hexagonMap = GetNode<Node2D>("/root/GameNode/Map/HexagonalMap");
        
        var selectionRectangle = GetNode<Line2D>("SelectionRectangle");

        if (_gameUserInterface.SelectionRectangle == null)
        {
            selectionRectangle.Visible = false;
        }
        else
        {
            selectionRectangle.Visible = true;
            var transform = hexagonMap.GetGlobalTransformWithCanvas();
            var rectangle = _gameUserInterface.SelectionRectangle.Value;
            switch (_gameUserInterface.SelectionOption)
            {
                case SelectionOptions.Rectangle:
                    selectionRectangle.Points
                        = new[]
                        {
                            transform * new Vector2(rectangle.Position.X, rectangle.Position.Y),
                            transform * new Vector2(rectangle.Position.X, rectangle.End.Y),
                            transform * new Vector2(rectangle.End.X, rectangle.End.Y),
                            transform * new Vector2(rectangle.End.X, rectangle.Position.Y),
                            transform * new Vector2(rectangle.Position.X, rectangle.Position.Y),
                        };
                    break;
                case SelectionOptions.Line:
                    selectionRectangle.Points
                        = new[]
                        {
                            transform * new Vector2(rectangle.Position.X, rectangle.Position.Y),
                            transform * new Vector2(rectangle.End.X, rectangle.End.Y),
                        };
                    break;
                case SelectionOptions.HexagonRing:
                case SelectionOptions.HexagonArea:
                    selectionRectangle.Visible = false;
                    break;
            }
        }

        base._Process(delta);
    }

    public override void _Input(InputEvent evnt)
    {
        if (evnt.IsPressed())
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
            else if (evnt.IsAction("pause"))
            {
                _gameUserInterface.TogglePause();
            }
        }

        base._Input(evnt);
    }

    private void OnTpsChanged(TpsChanged tpsChanged)
    {
        var tpsLabel = GetNode<Label>("TopMenuArea/Container/TpsLabel");
        tpsLabel.Text = tpsChanged.Tps.ToString("F0");
    }

    private void OnSpeedChanged(SpeedChanged speedChanged)
    {
        var speedLabel = GetNode<Label>("TopMenuArea/Container/SpeedLabel");

        String label;
        if (speedChanged.Paused)
        {
            label = "paused";
        }
        else
        {
            label = speedChanged.Speed + "x";
        }

        speedLabel.Text = label;
    }
    
    private void _on_SelectionButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;

        ToolLabel.Text = "Selection";

        _gameUserInterface.SelectionSelected();
    }
    
    private void _on_ConstructionButton_pressed()
    {
        PlaceItemContainer.Visible = false;
        DesignationContainer.Visible = false;
        DebugContainer.Visible = false;
        BuildingContainer.Visible = false;
        CraftingContainer.Visible = false;
        ConstructionContainer.Visible = !ConstructionContainer.Visible;
    }
    
    private void _on_PlaceItemButton_pressed()
    {
        ConstructionContainer.Visible = false;
        DesignationContainer.Visible = false;
        DebugContainer.Visible = false;
        BuildingContainer.Visible = false;
        CraftingContainer.Visible = false;
        PlaceItemContainer.Visible = !PlaceItemContainer.Visible;
    }

    private void _on_DesignationButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;
        DebugContainer.Visible = false;
        BuildingContainer.Visible = false;
        CraftingContainer.Visible = false;
        DesignationContainer.Visible = !DesignationContainer.Visible;
    }

    private void _on_DebugButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;
        DebugContainer.Visible = !DebugContainer.Visible;
        DesignationContainer.Visible = false;
        BuildingContainer.Visible = false;
        CraftingContainer.Visible = false;
    }
    
    private void _on_BuildingButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;
        DebugContainer.Visible = false;
        DesignationContainer.Visible = false;
        CraftingContainer.Visible = false;
        BuildingContainer.Visible = !BuildingContainer.Visible;
    }
    
    private void _on_CraftingButton_pressed()
    {
        ConstructionContainer.Visible = false;
        PlaceItemContainer.Visible = false;
        DebugContainer.Visible = false;
        BuildingContainer.Visible = false;
        CraftingContainer.Visible = !CraftingContainer.Visible;
        DesignationContainer.Visible = false;
    }



    public void _on_ConstructionSelectionButton_pressed(ConstructionDefinition definition)
    {
        _gameUserInterface.ConstructionSelected(definition);
        ToolLabel.Text = "Construction: " + definition.Label;
    }

    public void _on_PlaceItemSelectionButton_pressed(ItemDefinition itemDefinition)
    {
        _gameUserInterface.PlaceItemSelected(itemDefinition);
        ToolLabel.Text = "Place item";
    }

    public void _on_BuildingSelectionButton_pressed(BuildingDefinition definition)
    {
        _gameUserInterface.PlaceBuildingSelected(definition);
        ToolLabel.Text = "Place building: " + definition.Label;
    }

    public void _on_rotation_selected(long index)
    {
        _gameUserInterface.ConstructionRotation = (HexagonDirection)index;
    }
    
    private void _on_selectionOption(long index)
    {
        var selectionOption = (SelectionOptions)index;
        _gameUserInterface.SelectionOption = selectionOption;
    }
}