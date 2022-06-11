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
    private Texture _wallTexture;
    private Texture _tileTexture;
    
    public override void _Ready()
    {
        _grassTexture = ResourceLoader.Load<Texture>("res://Textures/Surface/grass.png");
        _dirtTexture = ResourceLoader.Load<Texture>("res://Textures/Surface/dirt.jpg");
        _tileTexture = ResourceLoader.Load<Texture>("res://Textures/Surface/TilePatternEven_Floor.png");
        _wallTexture = ResourceLoader.Load<Texture>("res://Textures/Wall/wall texture.svg");

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

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            RegenerateSurfaceMesh(surface, cells, mesh);
            System.Diagnostics.Debug.WriteLine("Regenerating surface: " + surface.Id + " took: " + watch.ElapsedMilliseconds);
        }

        Update();
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
        else
        {
            textureScale = 1;
        }

        var isWallSurface = surface == SurfaceDefinitions.RockWall || surface == SurfaceDefinitions.StoneWall ||
                            surface == SurfaceDefinitions.WoodenWall;

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

                int textureTriangle = triangle;
                Vector2 textureHex = new Vector2(1, 1);
                var currentHexColor = hexColor;

                if (isWallSurface)
                {
                    HexagonDirection directNeighborDirection;
                    HexagonDirection leftNeighborDirection;
                    HexagonDirection rightNeighborDirection;
                    if (triangle == 0)
                    {
                        leftNeighborDirection = HexagonDirection.TopRight;
                        directNeighborDirection = HexagonDirection.Right;
                        rightNeighborDirection = HexagonDirection.BottomRight;
                    }
                    else if (triangle == 1)
                    {
                        leftNeighborDirection = HexagonDirection.Right;
                        directNeighborDirection = HexagonDirection.BottomRight;
                        rightNeighborDirection = HexagonDirection.BottomLeft;
                    }
                    else if (triangle == 2)
                    {
                        leftNeighborDirection = HexagonDirection.BottomRight;
                        directNeighborDirection = HexagonDirection.BottomLeft;
                        rightNeighborDirection = HexagonDirection.Left;
                    }
                    else if (triangle == 3)
                    {
                        leftNeighborDirection = HexagonDirection.BottomLeft;
                        directNeighborDirection = HexagonDirection.Left;
                        rightNeighborDirection = HexagonDirection.TopLeft;
                    }
                    else if (triangle == 4)
                    {
                        leftNeighborDirection = HexagonDirection.Left;
                        directNeighborDirection = HexagonDirection.TopLeft;
                        rightNeighborDirection = HexagonDirection.TopRight;
                    }
                    else if (triangle == 5)
                    {
                        leftNeighborDirection = HexagonDirection.TopLeft;
                        directNeighborDirection = HexagonDirection.TopRight;
                        rightNeighborDirection = HexagonDirection.Right;
                    }
                    else
                    {
                        throw new InvalidOperationException("No hexagon triangle with such intex exists.");
                    }

                    var isDirectNeighborWall = IsNeighborWall(cell.Position + directNeighborDirection);
                    var isLeftNeighborWall = IsNeighborWall(cell.Position + leftNeighborDirection);
                    var isRightNeighborWall = IsNeighborWall(cell.Position + rightNeighborDirection);

                    if (isDirectNeighborWall && isLeftNeighborWall && isRightNeighborWall)
                    {
                        textureHex = new Vector2(2, 8.5f);
                    }
                    else if (triangle == 0 && isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 1 && !isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 2 && !isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 3 && !isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 4 && !isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 5 && !isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 0 && !isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 1);
                    }
                    else if (triangle == 1 && !isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 1);
                    }
                    else if (triangle == 2 && !isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 1);
                    }
                    else if (triangle == 3 && isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 1);
                    }
                    else if (triangle == 4 && !isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 1);
                    }
                    else if (triangle == 5 && !isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 1);
                    }
                    else if (triangle == 1 && !isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(6, 1.5f);
                    }
                    else if (triangle == 2 && isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(6, 1.5f);
                    }
                    else if (triangle == 3 && !isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(6, 1.5f);
                    }
                    else if (triangle == 0 && !isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(5, 3);
                    }
                    else if (triangle == 4 && !isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(5, 3);
                    }
                    else if (triangle == 5 && isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(5, 3);
                    }
                    else if (triangle == 0 && !isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(9, 1);
                    }
                    else if (triangle == 1 && isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(9, 1);
                    }
                    else if (triangle == 2 && !isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(9, 1);
                    }
                    else if (triangle == 3 && !isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(10, 2.5f);
                    }
                    else if (triangle == 4 && isDirectNeighborWall && !isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(10, 2.5f);
                    }
                    else if (triangle == 5 && !isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(10, 2.5f);
                    }
                    else if (triangle == 0 && isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 6);
                    }
                    else if (triangle == 5 && isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 6);
                    }
                    else if (triangle == 3 && isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 6);
                    }
                    else if (triangle == 4 && isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 6);
                    }
                    else if (triangle == 1 && isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(2, 4.5f);
                    }
                    else if (triangle == 2 && isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(2, 4.5f);
                    }
                    else if (triangle == 0 && isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(7, 5);
                    }
                    else if (triangle == 1 && isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(7, 5);
                    }
                    else if (triangle == 2 && isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(9, 5);
                    }
                    else if (triangle == 3 && isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(9, 5);
                    }
                    else if (triangle == 4 && isDirectNeighborWall && isRightNeighborWall && !isLeftNeighborWall)
                    {
                        textureHex = new Vector2(8, 6.5f);
                    }
                    else if (triangle == 5 && isDirectNeighborWall && !isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(8, 6.5f);
                    }
                    // stars
                    else if (triangle == 0 && !isDirectNeighborWall && isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(9, 1);
                    }
                    else if (triangle == 1 && !isDirectNeighborWall && isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 2 && !isDirectNeighborWall && isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else if (triangle == 3 && !isDirectNeighborWall && isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(6, 1.5f);
                    }
                    else if (triangle == 4 && !isDirectNeighborWall && isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(3, 1);
                    }
                    else if (triangle == 5 && !isDirectNeighborWall && isRightNeighborWall && isLeftNeighborWall)
                    {
                        textureHex = new Vector2(1, 1);
                    }
                    else
                    {
                        currentHexColor = Colors.Red;
                    }
                }

                colors___[triangleIndex * 3 + 0] = currentHexColor;
                colors___[triangleIndex * 3 + 1] = currentHexColor;
                colors___[triangleIndex * 3 + 2] = currentHexColor;

                if (isWallSurface)
                {
                    var textureHexCornerA = textureTriangle;
                    var textureHexCornerB = (textureTriangle + 1) % 6;

                    float textureWidth = 648.000f / 64f;
                    float textureHeight = 648.000f / 64f;
                    var halfHex = new Vector2(Mathf.Sqrt(3), 2) / 2;
                    Vector2 hexCenter = halfHex * textureHex + new Vector2(Mathf.Sqrt(3), 2) / 32;
                    Vector2 multiplier = new Vector2(1 / textureWidth, 1 / textureHeight);

                    textureUv[triangleIndex * 3 + 0] = (HexCubeCoord.Zero.Center(1) + hexCenter) * multiplier;
                    textureUv[triangleIndex * 3 + 1] =
                        (HexCubeCoord.HexCorner(HexCubeCoord.Zero, 1, textureHexCornerA) + hexCenter) * multiplier;
                    textureUv[triangleIndex * 3 + 2] =
                        (HexCubeCoord.HexCorner(HexCubeCoord.Zero, 1, textureHexCornerB) + hexCenter) * multiplier;
                }
                else
                {
                    textureUv[triangleIndex * 3 + 0] = textureCenter;
                    textureUv[triangleIndex * 3 + 1] =
                        HexCubeCoord.HexCorner(cell.Position, textureScale, hexCornerA);
                    textureUv[triangleIndex * 3 + 2] =
                        HexCubeCoord.HexCorner(cell.Position, textureScale, hexCornerB);
                }
            }
        }

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)ArrayMesh.ArrayType.Max);
        arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices_;
        arrays[(int)ArrayMesh.ArrayType.TexUv] = textureUv;
        arrays[(int)ArrayMesh.ArrayType.Color] = colors___;

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
    }

    private bool IsNeighborWall(HexCubeCoord neighborHex)
    {
        bool isNeighborWall;
        if (_game.GameMap.IsWithinMap(neighborHex))
        {
            var neighborCell = _game.GameMap.CellForPosition(neighborHex);
            isNeighborWall = neighborCell.Surface == SurfaceDefinitions.RockWall ||
                             neighborCell.Surface == SurfaceDefinitions.StoneWall ||
                             neighborCell.Surface == SurfaceDefinitions.WoodenWall;
        }
        else
        {
            isNeighborWall = true;
        }

        return isNeighborWall;
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
            }else if (surface == SurfaceDefinitions.TileFloor)
            {
                texture = _tileTexture;
            }
            else if (surface == SurfaceDefinitions.RockWall || surface == SurfaceDefinitions.StoneWall || surface == SurfaceDefinitions.WoodenWall)
            {
                texture = _wallTexture;
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
