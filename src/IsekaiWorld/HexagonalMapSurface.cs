using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace IsekaiWorld;

public partial class HexagonalMapSurface : Node2D
{
    public MessagingEndpoint Messaging { get; }

    private readonly Dictionary<SurfaceDefinition, ArrayMesh> _surfaceMeshes = new();

    private Texture2D _grassTexture = null!;
    private Texture2D _dirtTexture = null!;
    private Texture2D _tileTexture = null!;
    private Texture2D _roughStone = null!;
    
    private Boolean _isDirty;

    private record MapCell(HexCubeCoord Position, SurfaceDefinition Surface);

    private List<MapCell> _mapCells = new();
    
    public HexagonalMapSurface()
    {
        TextureRepeat = TextureRepeatEnum.Enabled;
        Messaging = new MessagingEndpoint(MessageHandler);
    }

    private void MessageHandler(IEntityMessage mssg)
    {
        switch (mssg)
        {
            case SurfaceChanged msg:
                _mapCells = msg.MapCells.Select(c=>new MapCell(c.Position, c.Surface)).ToList();
                _isDirty = true;
                break;
        }
    }

    public override void _EnterTree()
    {
        var gameNode = GetNode<GameNode>("/root/GameNode");
        gameNode.RegisterMessaging(Messaging);

        base._EnterTree();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _grassTexture = ResourceLoader.Load<Texture2D>("res://Textures/Surface/grass.png");
        _dirtTexture = ResourceLoader.Load<Texture2D>("res://Textures/Surface/dirt.jpg");
        _tileTexture = ResourceLoader.Load<Texture2D>("res://Textures/Surface/TilePatternEven_Floor.png");
        _roughStone = ResourceLoader.Load<Texture2D>("res://Textures/Surface/RoughStone.png");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_isDirty)
        {
            RefreshSurfaces();
            _isDirty = false;
        }
    }

    public override void _Draw()
    {
        foreach (var pair in _surfaceMeshes)
        {
            var surface = pair.Key;
            var mesh = pair.Value;

            Texture2D? texture;

            if (surface == SurfaceDefinitions.Grass)
            {
                texture = _grassTexture;
            }
            else if (surface == SurfaceDefinitions.Dirt)
            {
                texture = _dirtTexture;
            }
            else if (surface == SurfaceDefinitions.TileFloor)
            {
                texture = _tileTexture;
            }
            else if (surface == SurfaceDefinitions.RoughStone)
            {
                texture = _roughStone;
            }
            else
            {
                texture = null;
            }

            DrawMesh(mesh, texture);
        }
    }

    private void RefreshSurfaces()
    {
        var surfaces = _mapCells.GroupBy(x => x.Surface);

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

            RegenerateSurfaceMesh(surface, cells, mesh);
        }
        
        QueueRedraw();
    }

    private void RegenerateSurfaceMesh(SurfaceDefinition surface, List<MapCell> cells, ArrayMesh mesh)
    {
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
        else if (surface == SurfaceDefinitions.TileFloor)
        {
            textureScale = 0.05f;
        }
        else if (surface == SurfaceDefinitions.RoughStone)
        {
            textureScale = 0.4f;
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

            for (int triangle = 0; triangle < 6; triangle++)
            {
                var triangleIndex = cellIndex * 6 + triangle;
                var hexCornerA = triangle;
                var hexCornerB = (triangle + 1) % 6;

                vertices_[triangleIndex * 3 + 0] = verticleCenter;
                vertices_[triangleIndex * 3 + 1] = HexCubeCoord.HexCorner(cell.Position, 1, hexCornerA);
                vertices_[triangleIndex * 3 + 2] = HexCubeCoord.HexCorner(cell.Position, 1, hexCornerB);

                colors___[triangleIndex * 3 + 0] = hexColor;
                colors___[triangleIndex * 3 + 1] = hexColor;
                colors___[triangleIndex * 3 + 2] = hexColor;

                textureUv[triangleIndex * 3 + 0] = textureCenter;
                textureUv[triangleIndex * 3 + 1] =
                    HexCubeCoord.HexCorner(cell.Position, textureScale, hexCornerA);
                textureUv[triangleIndex * 3 + 2] =
                    HexCubeCoord.HexCorner(cell.Position, textureScale, hexCornerB);
            }
        }

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = Variant.CreateFrom(vertices_);
        arrays[(int)Mesh.ArrayType.TexUV] = Variant.CreateFrom(textureUv);
        arrays[(int)Mesh.ArrayType.Color] = Variant.CreateFrom(colors___);

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
    }
}