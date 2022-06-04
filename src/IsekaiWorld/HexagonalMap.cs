using System;
using System.Collections.Generic;
using Godot;

public class HexagonalMap : Node2D
{
	private  GameEntity _game;
	private ArrayMesh _hexesMesh;
	
	private HexagonNode _mouseoverHexagon;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_game = new GameEntity();
		_game.Initialize();

		var adamCharacter = _game.AddCharacter("Adam");
		adamCharacter.Position = new HexCubeCoord(1, 1, -2);
		var eveCharacter = _game.AddCharacter("Eve");
		eveCharacter.Position = new HexCubeCoord(1, -1, 0);
		
		_hexesMesh = new ArrayMesh();

		foreach (var cell in _game.GameMap.Cells)
		{
			var center = cell.Position.Center(1);
			Vector2[] points = new Vector2[7];
			points[0] = center;
			for (int i = 0; i < 6; i++)
			{
				points[i + 1] = HexCubeCoord.HexCorner(cell.Position, 1, i);
			}

			var hexColor = cell.Surface.Color;

			Color[] colors = {
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

		_mouseoverHexagon = new HexagonNode
		{
			Color = Colors.Red
		};
		AddChild(_mouseoverHexagon);

		var uiNode = GetNode<CanvasLayer>("/root/Game/UI");
		uiNode.AddChild(new Label(){Text = "Test label"});
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
		}

		base._Input(@event);
	}

	enum Tool
	{
		Selection,
		Construction
	}

	private Tool _currentTool = Tool.Selection;
	
	public void _on_SelectionButton_pressed()
	{
		_currentTool = Tool.Selection;
	}
	
	public void _on_ConstructionButton_pressed()
	{
		_currentTool = Tool.Construction;
	}

	public override void _Process(float delta)
	{
		var position = GetLocalMousePosition();

		var hex = HexCubeCoord.FromPosition(position, 1);
		_mouseoverHexagon.HexPosition = hex;

		_game.Update(delta);

		_game.UpdateNodes(this);
		
		base._Process(delta);
	}

	private readonly Dictionary<object, Node> _nodeMappings = new Dictionary<object, Node>();
	
	public void AddNodeReference(object entity, Node node)
	{
		_nodeMappings.Add(entity, node);
	}

	public void RemoveNodeFor(object entity)
	{
		RemoveChild(_nodeMappings[entity]);
	}

	public TNode GetEntityNode<TNode>(object entity)
		where TNode : Node
	{
		return (TNode)_nodeMappings[entity];
	}
}
