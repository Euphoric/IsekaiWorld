using System.Collections.Generic;
using System.Linq;
using Godot;

public class HexagonalMap : Node2D
{
    private HexMap _map;
    private ArrayMesh _hexesMesh;

    private readonly float _hexSize = 16;
    private HexagonNode _mouseoverHexagon;
    private HexagonNode _characterHexagon;
    private HexagonNode _selectionHexagon;
    private Line2D _characterPath;

    private HexagonPathfinding _pathfinding;
    private CharaterEntity _characterEntity;
    private readonly List<ConstructionEntity> _constructionEntities = new List<ConstructionEntity>();

    private readonly List<IActivity> _activities = new List<IActivity>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _map = new HexMap(32);
        _map.GenerateMap();

        _hexesMesh = new ArrayMesh();
        _pathfinding = new HexagonPathfinding();
        _pathfinding.BuildMap(_map);

        foreach (var cell in _map.Cells)
        {
            var center = cell.Position.Center(_hexSize);
            Vector2[] points = new Vector2[7];
            points[0] = center;
            for (int i = 0; i < 6; i++)
            {
                points[i + 1] = HexCubeCoord.HexCorner(cell.Position, _hexSize, i);
            }

            var hexColor = cell.Surface.Color;

            Color[] colors = new[]
            {
                hexColor,
                hexColor,
                hexColor,
                hexColor,
                hexColor,
                hexColor,
                hexColor,
            };
            var indices = new[]
            {
                0, 1, 2, 3, 4, 5, 6, 1
            };
            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)ArrayMesh.ArrayType.Max);
            arrays[(int)ArrayMesh.ArrayType.Vertex] = points;
            arrays[(int)ArrayMesh.ArrayType.Index] = indices;
            arrays[(int)ArrayMesh.ArrayType.Color] = colors;
            _hexesMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleFan, arrays);
        }

        _selectionHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Scale = new Vector2(_hexSize, _hexSize),
            Color = Colors.White
        };
        AddChild(_selectionHexagon);

        _characterEntity = new CharaterEntity(this, _pathfinding)
        {
            Position = HexCubeCoord.Zero
        };
        _characterHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Scale = new Vector2(_hexSize, _hexSize),
            Color = Colors.Blue
        };
        AddChild(_characterHexagon);
        _characterPath = new Line2D
        {
            Width = 1,
            DefaultColor = Colors.Blue
        };
        AddChild(_characterPath);

        _mouseoverHexagon = new HexagonNode
        {
            Scale = new Vector2(_hexSize, _hexSize),
            Color = Colors.Red
        };
        AddChild(_mouseoverHexagon);
    }

    public override void _Draw()
    {
        DrawMesh(_hexesMesh, null);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == (int)ButtonList.Left && mouseButton.Pressed)
            {
                var clickPosition = _mouseoverHexagon.HexPosition;
                _selectionHexagon.HexPosition = clickPosition;

                //_characterEntity.MoveTo(_mouseoverHexagon.HexPosition);

                StartConstruction(clickPosition);
            }
        }

        base._Input(@event);
    }

    private void StartConstruction(HexCubeCoord position)
    {
        var constructionExists = _constructionEntities.Any(x => x.Position == position);
        if (!constructionExists)
        {
            var constructionEntity = new ConstructionEntity();
            constructionEntity.Position = position;
            _constructionEntities.Add(constructionEntity);

            var constructionNode = new HexagonNode
            {
                Color = Colors.MediumPurple,
                Scale = new Vector2(_hexSize, _hexSize),
            };
            constructionNode.HexPosition = constructionEntity.Position;
            AddChild(constructionNode);
            constructionEntity.Node = constructionNode;
        }
    }

    public override void _Process(float delta)
    {
        var position = GetLocalMousePosition();

        var hex = HexCubeCoord.FromPosition(position, _hexSize);
        _mouseoverHexagon.HexPosition = hex;

        if (_characterEntity.IsIdle && _constructionEntities.Any())
        {
            var firstConstruction = _constructionEntities.First();
            _characterEntity.Construct(firstConstruction);
        }

        _characterEntity.Update(delta);
        _characterEntity.UpdateCharacterNode(_characterHexagon);
        _characterEntity.UpdatePathNode(_characterPath);

        foreach (var activity in _activities)
        {
            activity.Update(delta);
        }

        foreach (var entity in _constructionEntities)
        {
            entity.UpdateNode();
        }
        
        _activities.RemoveAll(x => x.IsFinished);

        base._Process(delta);
    }

    public void RunActivity(IActivity activity)
    {
        _activities.Add(activity);
    }

    public void RemoveConstruction(ConstructionEntity construction)
    {
        _constructionEntities.Remove(construction);
        RemoveChild(construction.Node);
    }
}