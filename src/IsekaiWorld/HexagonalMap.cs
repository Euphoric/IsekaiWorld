using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HexagonalMap : Node2D
{
    private readonly Dictionary<SurfaceDefinition, ArrayMesh> _surfaceMeshes =
        new Dictionary<SurfaceDefinition, ArrayMesh>();

    private HexagonNode _mouseoverHexagon;
    private GameEntity _game;

    private Texture _grassTexture;
    private Texture _dirtTexture;

    public override void _Ready()
    {
        _grassTexture = ResourceLoader.Load<Texture>("res://Textures/Surface/grass.png");
        _dirtTexture = ResourceLoader.Load<Texture>("res://Textures/Surface/dirt.jpg");

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
        var surfaces = _game.GameMap.Cells.GroupBy(x => x.Surface);

        foreach (var surfaceGroup in surfaces)
        {
            var surface = surfaceGroup.Key;
            var cells = surfaceGroup.ToList();

            if (!_surfaceMeshes.TryGetValue(surface, out var mesh))
            {
                mesh = new ArrayMesh();
                _surfaceMeshes[surface] = mesh;
            }
            else
            {
                mesh.ClearSurfaces();
            }

            Color hexColor;
            if (surface == SurfaceDefinitions.Grass || surface == SurfaceDefinitions.Dirt)
            {
                hexColor = Colors.White;
            }
            else
            {
                hexColor = surface.Color;
            }

            float textureScale;
            if (surface == SurfaceDefinitions.Grass)
            {
                textureScale = 0.2f;
            }
            else if (surface == SurfaceDefinitions.Dirt)
            {
                textureScale = 1.0f;
            }
            else
            {
                textureScale = 1;
            }

// ReSharper disable InconsistentNaming
            var verticesCount = 3 * 6 * cells.Count;
            Vector2[] vertices_ = new Vector2[verticesCount];
            Vector2[] textureUv = new Vector2[verticesCount];
            Color[] colors___ = new Color[verticesCount];
// ReSharper restore InconsistentNaming

            for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
            {
                var cell = cells[cellIndex];

                var verticleCenter = cell.Position.Center(1);
                var textureCenter = cell.Position.Center(textureScale);

                for (int i = 0; i < 6; i++)
                {
                    var triangleIndex = cellIndex * 6 + i;
                    var hexCornerA = i;
                    var hexCornerB = (i + 1) % 6;
                    
                    vertices_[triangleIndex * 3 + 0] = verticleCenter;
                    textureUv[triangleIndex * 3 + 0] = textureCenter;
                    colors___[triangleIndex * 3 + 0] = hexColor;
                    vertices_[triangleIndex * 3 + 1] = HexCubeCoord.HexCorner(cell.Position, 1, hexCornerA);
                    textureUv[triangleIndex * 3 + 1] = HexCubeCoord.HexCorner(cell.Position, textureScale, hexCornerA);
                    colors___[triangleIndex * 3 + 1] = hexColor;
                    vertices_[triangleIndex * 3 + 2] = HexCubeCoord.HexCorner(cell.Position, 1, hexCornerB);
                    textureUv[triangleIndex * 3 + 2] = HexCubeCoord.HexCorner(cell.Position, textureScale, hexCornerB);
                    colors___[triangleIndex * 3 + 2] = hexColor;
                }
            }

            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)ArrayMesh.ArrayType.Max);
            arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices_;
            arrays[(int)ArrayMesh.ArrayType.TexUv] = textureUv;
            arrays[(int)ArrayMesh.ArrayType.Color] = colors___;

            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        }

        Update();
    }

    public override void _Draw()
    {
        foreach (var pair in _surfaceMeshes)
        {
            var surface = pair.Key;
            var mesh = pair.Value;

            Texture texture = null;

            if (surface == SurfaceDefinitions.Grass)
            {
                texture = _grassTexture;
            }
            else if (surface == SurfaceDefinitions.Dirt)
            {
                texture = _dirtTexture;
            }

            DrawMesh(mesh, texture);
        }
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