using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using IsekaiWorld.Game;

namespace IsekaiWorld.View;

public class GameUserInterface
{
    public MessagingEndpoint Messaging { get; }

    public GameUserInterface()
    {
        Messaging = new MessagingEndpoint(HandleMessage);
    }

    public abstract class EntitySelection
    {
        public string Id { get; }

        protected EntitySelection(String id)
        {
            Id = id;
        }

        protected void Update(HexCubeCoord position)
        {
            Position = position;
        }

        public HexCubeCoord Position { get; set; }
        public string TextLabel { get; protected set; } = "";
    }

    private class ConstructionEntityItem : EntitySelection
    {
        public ConstructionEntityItem(string id) : base(id)
        {
        }

        public void Update(ConstructionUpdated msg)
        {
            base.Update(msg.Position);
            Definition = msg.Definition;
            ProgressRelative = msg.ProgressRelative;
            
            var progress = Godot.Mathf.FloorToInt(ProgressRelative * 100);
            TextLabel = $"Construction: {Definition.Label} Progress: {progress}";
        }

        public ConstructionDefinition Definition { get; set; } = null!;

        public float ProgressRelative { get; set; }
    }

    private class BuildingEntityItem : EntitySelection
    {
        public BuildingEntityItem(string id) : base(id)
        {
        }

        public void Update(BuildingUpdated msg)
        {
            base.Update(msg.Position);
            Definition = msg.Definition;
            TextLabel = "Building: " + Definition.Label;
        }

        public BuildingDefinition Definition { get; set; } = null!;
    }

    private class CharacterSelection : EntitySelection
    {
        public CharacterSelection(string id) : base(id)
        {
        }

        public void Update(CharacterUpdated msg)
        {
            base.Update(msg.Position);
            Label = msg.Label;
            ActivityName = msg.ActivityName;
            Hunger = msg.Hunger;
            
            TextLabel = $"Character: {Label} / Activity: {ActivityName} / Hunger: {Hunger * 100:F1}";
        }

        public double Hunger { get; set; }

        public string? ActivityName { get; set; }

        public string Label { get; set; } = null!;
    }

    private class ItemEntityItem : EntitySelection
    {
        public ItemEntityItem(string id) : base(id)
        {
        }

        public void Update(ItemUpdated msg)
        {
            base.Update(msg.Position);
            Definition = msg.Definition;
            Count = msg.Count;
            
            TextLabel = $"Item: {Definition.Label} Count: {Count}";
        }

        public int Count { get; set; }

        public ItemDefinition Definition { get; set; } = null!;
    }

    private readonly Dictionary<String, EntitySelection> _selectionEntities = new();

    private void UpdateSelectionEntity<TMsg, TSelection>(TMsg msg, Func<TMsg, String> id,
        Func<String, TSelection> newFunc, Action<TMsg, TSelection> update)
        where TSelection : EntitySelection
    {
        var updatedEntity = _selectionEntities.GetOrAdd(id(msg), newFunc);
        update(msg, (TSelection)updatedEntity);

        if (CurrentSelection == updatedEntity)
        {
            OnCurrentSelectionChanged?.Invoke(CurrentSelection);
        }
    }

    private void HandleMessage(IEntityMessage mssg)
    {
        switch (mssg)
        {
            case BuildingUpdated msg:
                UpdateSelectionEntity(msg, m => m.EntityId,
                    id => new BuildingEntityItem(id),
                    (m, s) => s.Update(m));
                break;
            case BuildingRemoved msg:
                _selectionEntities.Remove(msg.EntityId);
                break;
            case CharacterUpdated msg:
                UpdateSelectionEntity(msg, m => m.EntityId,
                    id => new CharacterSelection(id),
                    (m, s) => s.Update(m));
                break;
            case ConstructionUpdated msg:
                UpdateSelectionEntity(msg, m => m.EntityId,
                    id => new ConstructionEntityItem(id),
                    (m, s) => s.Update(m));
                break;
            case ConstructionRemoved msg:
                _selectionEntities.Remove(msg.EntityId);
                break;
            case ItemUpdated msg:
                UpdateSelectionEntity(msg, m => m.EntityId,
                    id => new ItemEntityItem(id),
                    (m, s) => s.Update(m));
                break;
            case ItemPickedUp msg:
                _selectionEntities.Remove(msg.EntityId);
                break;
            case ItemRemoved msg:
                _selectionEntities.Remove(msg.EntityId);
                break;
        }
    }

    public HexagonDirection ConstructionRotation { get; set; }

    public void Update()
    {

    }

    private void SelectItemOn(HexCubeCoord position)
    {
        var selectedEntity = _selectionEntities.Values.FirstOrDefault(x => x.Position == position);

        if (selectedEntity != null)
        {
            CurrentSelection = selectedEntity;
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
    private ConstructionDefinition? _currentConstructionSelection;
    private BuildingDefinition? _currentBuildingSelection;
    private ItemDefinition? _currentItemSelection;
    private DesignationDefinition? _currentDesignation;
    private EntitySelection? _currentSelection;

    public Vector2 MousePosition { get; private set; }
    public HexCubeCoord MouseHexPosition { get; private set; }
    public Rect2? SelectionRectangle { get; private set; }
    public List<HexCubeCoord> HighlightedHexes { get; } = new();
    public SelectionOptions SelectionOption { get; set; }

    public EntitySelection? CurrentSelection
    {
        get => _currentSelection;
        set { _currentSelection = value; OnCurrentSelectionChanged?.Invoke(_currentSelection); }
    }

    public event Action<EntitySelection?>? OnCurrentSelectionChanged; 

    public void MouseClickOnMap(bool isPressed)
    {
        if (isPressed)
        {
            SelectionRectangle = new Rect2(MousePosition, Vector2.Zero);
        }
        else
        {
            SelectionRectangle = null;

            ApplyCurrentTool();
        }
    }

    private void ApplyCurrentTool()
    {
        switch (_currentTool)
        {
            case Tool.Selection:
                SelectItemOn(MouseHexPosition);
                break;
            case Tool.Construction:
                foreach (var hexPosition in HighlightedHexes)
                {
                    var msg = new StartConstruction(hexPosition, ConstructionRotation, _currentConstructionSelection!);
                    Messaging.Broadcast(msg);
                }

                break;
            case Tool.PlaceBuilding:
                foreach (var hexPosition in HighlightedHexes)
                {
                    var msg = new SpawnBuilding(hexPosition, ConstructionRotation, _currentBuildingSelection!);
                    Messaging.Broadcast(msg);
                }

                break;
            case Tool.PlaceItem:
                Messaging.Broadcast(new SpawnItem(MouseHexPosition, _currentItemSelection!, 1));
                break;
            case Tool.Designate:
                foreach (var hexPosition in HighlightedHexes)
                {
                    Messaging.Broadcast(new Designate(hexPosition, _currentDesignation!));
                }

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
        _currentConstructionSelection = definition;
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

    public void DesignateTool(DesignationDefinition designation)
    {
        _currentTool = Tool.Designate;
        _currentDesignation = designation;
    }

    public void SetSpeed(int speed)
    {
        Messaging.Broadcast(new SetSpeed(speed));
    }

    public void TogglePause()
    {
        Messaging.Broadcast(new TogglePause());
    }

    public void SetCharacterHealth()
    {
        if (CurrentSelection is CharacterSelection cs)
        {
            Messaging.Broadcast(new SetCharacterHunger(cs.Id, 0.31));
        }
    }

    public void MousePositionChanged(Vector2 mousePosition)
    {
        MousePosition = mousePosition;
        MouseHexPosition = HexCubeCoord.FromPosition(mousePosition, 1);

        HighlightedHexes.Clear();
        if (SelectionRectangle != null)
        {
            var rect = SelectionRectangle.Value;
            rect.End = mousePosition;
            SelectionRectangle = rect;

            var fromHex = HexCubeCoord.FromPosition(rect.Position, 1);
            var toHex = HexCubeCoord.FromPosition(rect.End, 1);
            switch (SelectionOption)
            {
                case SelectionOptions.Rectangle:
                    HexCubeCoord.FillRectangleBetweenHexes(fromHex, toHex, HighlightedHexes);
                    break;
                case SelectionOptions.Line:
                    HexCubeCoord.LineBetweenHexes(fromHex, toHex, HighlightedHexes);
                    break;
                case SelectionOptions.HexagonRing:
                {
                    var radius = HexCubeCoord.Distance(fromHex, toHex);
                    HighlightedHexes.AddRange(HexCubeCoord.HexagonRing(fromHex, radius));
                    break;
                }
                case SelectionOptions.HexagonArea:
                {
                    var radius = HexCubeCoord.Distance(fromHex, toHex);
                    HighlightedHexes.AddRange(HexCubeCoord.HexagonArea(fromHex, radius));
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void AddCraftingBill(CraftingDefinition craftingDefinition)
    {
       Messaging.Broadcast(new AddCraftingBill(craftingDefinition)); 
    }
}