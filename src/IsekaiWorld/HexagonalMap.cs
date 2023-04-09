using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace IsekaiWorld;

public partial class HexagonalMap : Node2D
{
    public MessagingEndpoint Messaging { get; }

    private readonly Dictionary<BuildingDefinition, ArrayMesh> _buildingMeshes = new();

    private readonly Dictionary<BuildingDefinition, Texture2D> _buildingTextures = new();

    private bool _isDirty;
    private HexagonNode _mouseoverHexagon = null!;
    [Obsolete("Use messaging")]
    private GameEntity _game = null!;

    public HexagonalMap()
    {
        TextureRepeat = TextureRepeatEnum.Enabled;
        Messaging = new MessagingEndpoint(MessageHandler);
    }

    public override void _Ready()
    {
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

    private void RefreshGameMap()
    {
        _buildingTextures.Clear();

        var wallBuildings = _game.Buildings.Where(e => e.Definition.EdgeConnected).ToList();
        var connectedPositions = wallBuildings.Select(x => x.Position).ToHashSet();
        foreach (var kvp in wallBuildings.GroupBy(x => x.Definition))
        {
            var definition = kvp.Key;
            var buildings = kvp.ToList();

            if (!_buildingMeshes.TryGetValue(definition, out var mesh))
            {
                mesh = new ArrayMesh();
                _buildingMeshes[definition] = mesh;
            }
            else
            {
                mesh.ClearSurfaces();
            }

            RegenerateConnectedBuildingMesh(definition, buildings.Select(b => b.Position).ToList(), connectedPositions,
                mesh);
            _buildingTextures[definition] =
                ResourceLoader.Load<Texture2D>(definition.TextureResource[HexagonDirection.Right]);
        }

        QueueRedraw();
    }

    private void RegenerateConnectedBuildingMesh(BuildingDefinition building, List<HexCubeCoord> positions,
        ISet<HexCubeCoord> connectedPositions, ArrayMesh mesh)
    {
        Color hexColor = building.Color;

        // ReSharper disable InconsistentNaming
        var verticesCount = 3 * 6 * positions.Count;
        Vector2[] vertices_ = new Vector2[verticesCount];
        Vector2[] textureUv = new Vector2[verticesCount];
        Color[] colors___ = new Color[verticesCount];
// ReSharper restore InconsistentNaming

        for (int cellIndex = 0; cellIndex < positions.Count; cellIndex++)
        {
            var position = positions[cellIndex];

            var verticleCenter = position.Center(1);

            for (int triangle = 0; triangle < 6; triangle++)
            {
                var triangleIndex = cellIndex * 6 + triangle;
                var hexCornerA = triangle;
                var hexCornerB = (triangle + 1) % 6;

                vertices_[triangleIndex * 3 + 0] = verticleCenter;
                vertices_[triangleIndex * 3 + 1] = HexCubeCoord.HexCorner(position, 1, hexCornerA);
                vertices_[triangleIndex * 3 + 2] = HexCubeCoord.HexCorner(position, 1, hexCornerB);

                int textureTriangle = triangle;
                Vector2 textureHex = new Vector2(1, 1);
                var currentHexColor = hexColor;

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

                var isDirectNeighborWall = IsNeighborWall(connectedPositions, position + directNeighborDirection);
                var isLeftNeighborWall = IsNeighborWall(connectedPositions, position + leftNeighborDirection);
                var isRightNeighborWall = IsNeighborWall(connectedPositions, position + rightNeighborDirection);

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


                colors___[triangleIndex * 3 + 0] = currentHexColor;
                colors___[triangleIndex * 3 + 1] = currentHexColor;
                colors___[triangleIndex * 3 + 2] = currentHexColor;


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
        }

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = Variant.CreateFrom(vertices_);
        arrays[(int)Mesh.ArrayType.TexUV] = Variant.CreateFrom(textureUv);
        arrays[(int)Mesh.ArrayType.Color] = Variant.CreateFrom(colors___);

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
    }

    private bool IsNeighborWall(ISet<HexCubeCoord> connectedPositions, HexCubeCoord neighborHex)
    {
        return connectedPositions.Contains(neighborHex);
    }

    public override void _Draw()
    {
        foreach (var kvp in _buildingMeshes)
        {
            var definition = kvp.Key;
            var mesh = kvp.Value;

            DrawMesh(mesh, _buildingTextures[definition]);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                var clickPosition = _mouseoverHexagon.HexPosition;
                _game.UserInterface.MouseClickOnMap(clickPosition);
            }
        }

        base._Input(@event);
    }

    public override void _Process(double delta)
    {
        var position = GetLocalMousePosition();

        var hex = HexCubeCoord.FromPosition(position, 1);
        _mouseoverHexagon.HexPosition = hex;

        if (_isDirty)
        {
            RefreshGameMap();
            _isDirty = false;
        }

        base._Process(delta);
    }

    private void MessageHandler(IEntityMessage message)
    {
        switch (message)
        {
            case BuildingUpdated bu:
                if (bu.Definition.EdgeConnected)
                {
                    _isDirty = true;
                }

                break;
            case BuildingRemoved br:
                if (br.Definition.EdgeConnected)
                {
                    _isDirty = true;
                }

                break;
        }
    }
}