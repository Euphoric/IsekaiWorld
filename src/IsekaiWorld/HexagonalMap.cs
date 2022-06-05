using System;
using System.Collections.Generic;
using Godot;

public class HexagonalMap : Node2D
{
	private ArrayMesh _hexesMesh;
	private ArrayMesh _grassSurfaceMesh;
	private ArrayMesh _dirtSurfaceMesh;
	
	private HexagonNode _mouseoverHexagon;
	private GameEntity _game;
	
	private Texture _grassTexture;
	private Texture _dirtTexture;

	public override void _Ready()
	{
		_grassTexture = ResourceLoader.Load<Texture>("res://Textures/Surface/grass.png");
		_dirtTexture = ResourceLoader.Load<Texture>("res://Textures/Surface/dirt.jpg");
		
		_hexesMesh = new ArrayMesh();
		_grassSurfaceMesh = new ArrayMesh();
		_dirtSurfaceMesh = new ArrayMesh();
		
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
		_grassSurfaceMesh.ClearSurfaces();
		_dirtSurfaceMesh.ClearSurfaces();
		_hexesMesh.ClearSurfaces();
		foreach (var cell in _game.GameMap.Cells)
		{
			float textureScale;
			
			if (cell.Surface == SurfaceDefinitions.Grass)
			{
				textureScale = 0.2f;
			}
			else if (cell.Surface == SurfaceDefinitions.Dirt)
			{
				textureScale = 1.0f;
			}
			else
			{
				textureScale = 1;
			}
			
			Vector2[] vertices = new Vector2[7];
			Vector2[] textureUv = new Vector2[7];
			vertices[0] = cell.Position.Center(1);
			textureUv[0] = cell.Position.Center(textureScale);
			for (int i = 0; i < 6; i++)
			{
				vertices[i + 1] = HexCubeCoord.HexCorner(cell.Position, 1, i);
				textureUv[i + 1] = HexCubeCoord.HexCorner(cell.Position, textureScale, i);
			}

			Color hexColor;

			if (cell.Surface == SurfaceDefinitions.Grass || cell.Surface == SurfaceDefinitions.Dirt)
			{
				hexColor = Colors.White;
			}
			else
			{
				hexColor = cell.Surface.Color;
			}
				

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
			arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
			arrays[(int)ArrayMesh.ArrayType.TexUv] = textureUv;
			arrays[(int)ArrayMesh.ArrayType.Index] = indices;
			arrays[(int)ArrayMesh.ArrayType.Color] = colors;

			if (cell.Surface == SurfaceDefinitions.Grass)
			{
				_grassSurfaceMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleFan, arrays);
			}
			else if (cell.Surface == SurfaceDefinitions.Dirt)
			{
				_dirtSurfaceMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleFan, arrays);				
			}
			else
			{
				_hexesMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleFan, arrays);                
			}
		}
	}

	public override void _Draw()
	{
		DrawMesh(_hexesMesh, null);
		DrawMesh(_grassSurfaceMesh, _grassTexture);
		DrawMesh(_dirtSurfaceMesh, _dirtTexture);
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
