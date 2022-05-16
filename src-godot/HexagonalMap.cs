using Godot;
using System;

public class CharacterNode : Node2D
{
	private HexCubeCoord _hexPosition;

	public HexCubeCoord HexPosition
	{
		get => _hexPosition;
		set
		{
			_hexPosition = value;
			Position = _hexPosition.Center(16);
		}
	}

	public CharacterNode()
	{
		var polygon = new[]
		{
			new Vector2(1, 1),
			new Vector2(1, -1),
			new Vector2(-1, -1),
			new Vector2(-1, 1)
		};
		var polygonNode = new Polygon2D
		{
			Polygon = polygon,
		};
		AddChild(polygonNode);
	}
}

public class HexagonalMap : Node2D
{
	private HexMap _map;
	private ArrayMesh _hexesMesh;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = new HexMap(32);
		_map.GenerateMap();
		
		var hexSize = 16;
		_hexesMesh = new ArrayMesh();
		
		foreach (var cell in _map.Cells)
		{
			var center = cell.Position.Center(hexSize);
			Vector2[] points = new Vector2[7];
			points[0] = center;
			for (int i = 0; i < 6; i++)
			{
				points[i + 1] = flat_hex_corner(cell.Position, hexSize, i);
			}

			var hexColor = cell.Surface == 1
				? Colors.DarkGreen
				: Colors.SaddleBrown;

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
			var indices = new int[]
			{
				0, 1, 2, 3, 4, 5, 6, 1
			};
			var arrays = new Godot.Collections.Array();
			arrays.Resize((int) ArrayMesh.ArrayType.Max);
			arrays[(int) ArrayMesh.ArrayType.Vertex] = points;
			arrays[(int) ArrayMesh.ArrayType.Index] = indices;
			arrays[(int) ArrayMesh.ArrayType.Color] = colors;
			_hexesMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleFan, arrays);
		}

		var characterNode = new CharacterNode
		{
			HexPosition = new HexCubeCoord(3, -1, -2),
			Scale = new Vector2(hexSize, hexSize)
		};
		AddChild(characterNode);
	}

	public override void _Draw()
	{
		DrawMesh(_hexesMesh, null);
	}

	Vector2 flat_hex_corner(HexCubeCoord coordinates, float size, int i)
	{
		var center = coordinates.Center(size);

		var angleDeg = 60 * i - 30;
		var angleRad = (float) (Math.PI / 180 * angleDeg);
		return new Vector2(center.x + size * Mathf.Cos(angleRad),
			center.y + size * Mathf.Sin(angleRad));
	}
}
