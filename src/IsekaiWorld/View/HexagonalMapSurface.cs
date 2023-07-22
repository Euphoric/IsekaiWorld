using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using IsekaiWorld.Game;

namespace IsekaiWorld.View;

public partial class HexagonalMapSurface : Node2D
{
    public MessagingEndpoint Messaging { get; }

    private class SurfaceData
    {
        public ArrayMesh Mesh { get; set; } = null!;
        public Texture2D? Texture { get; set; }
    }
    
    private readonly Dictionary<SurfaceDefinition, SurfaceData> _surfaceMeshes = new();

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
        var gameNode = GetNode<View.GameNode>("/root/GameNode");
        gameNode.RegisterMessaging(Messaging);

        base._EnterTree();
    }
    
    
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
        foreach (var (_, data) in _surfaceMeshes)
        {
            DrawMesh(data.Mesh, data.Texture);
        }
    }

    private void RefreshSurfaces()
    {
        var surfaces = _mapCells.GroupBy(x => x.Surface);

        foreach (var surfaceGroup in surfaces)
        {
            var surface = surfaceGroup.Key;
            var cells = surfaceGroup.ToList();

            if (!_surfaceMeshes.TryGetValue(surface, out var data))
            {
                data = new SurfaceData
                {
                    Mesh = new ArrayMesh(),
                    Texture = surface.Texture?.Let(t => ResourceLoader.Load<Texture2D>(t))
                };
                _surfaceMeshes[surface] = data;
            }
            else
            {
                data.Mesh.ClearSurfaces();
            }

            RegenerateSurfaceMesh(surface, cells, data.Mesh);
        }
        
        QueueRedraw();
    }

    private void RegenerateSurfaceMesh(SurfaceDefinition surface, List<MapCell> cells, ArrayMesh mesh)
    {
        var hexColor = surface.Color;
        float textureScale = surface.TextureScale;

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