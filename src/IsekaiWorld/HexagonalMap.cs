using System;
using System.Collections.Generic;
using Godot;

public class HexagonalMap : Node2D
{
	private ArrayMesh _hexesMesh;
	private HexagonNode _mouseoverHexagon;
	private GameEntity _game;
	
	public override void _Ready()
	{
		_hexesMesh = new ArrayMesh();

		_mouseoverHexagon = new HexagonNode
		{
			Color = Colors.Red
		};
		AddChild(_mouseoverHexagon);
	}

	public override void _EnterTree()
	{
		var gameNode = GetNode<GameNode>("/root/GameNode");
		_game = gameNode.GameEntity;
		
		base._EnterTree();
	}

	public void RefreshGameMap()
	{
		_hexesMesh.ClearSurfaces();
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

			Color[] colors =
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
	}

	public override void _Draw()
	{
		DrawMesh(_hexesMesh, null);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int)ButtonList.Left && mouseButton.Pressed)
			{
				var clickPosition = _mouseoverHexagon.HexPosition;
				_game.UserInterface.MouseClickOnMap(clickPosition);
			}
		}

		base._Input(@event);
	}

	public override void _Process(float delta)
	{
		var position = GetLocalMousePosition();

		var hex = HexCubeCoord.FromPosition(position, 1);
		_mouseoverHexagon.HexPosition = hex;

		base._Process(delta);
	}
}
