using System;
using System.Collections.Generic;
using Godot;

public class GameNode : Node
{
    private  GameEntity _game;

    public GameEntity GameEntity => _game;
    public HexagonalMap MapNode { get; private set; }

    public override void _EnterTree()
    {
        _game = new GameEntity();

        base._EnterTree();
    }
    
    public override void _Ready()
    {
        var mapGenerator = new MapGenerator();
        //var mapGenerator = new WallTilingTestMapGenerator();
        _game.Initialize(mapGenerator);
        
        var adamCharacter = _game.AddCharacter("Adam");
        adamCharacter.Position = new HexCubeCoord(1, 1, -2);
        var eveCharacter = _game.AddCharacter("Eve");
        eveCharacter.Position = new HexCubeCoord(1, -1, 0);

        MapNode = GetNode<HexagonalMap>("Map/HexagonalMap");
        AddNodeReference(_game.GameMap, MapNode);
    }

    public override void _Process(float delta)
    {
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
        var nodeToRemove = _nodeMappings[entity];
        nodeToRemove.GetParent().RemoveChild(nodeToRemove);
    }

    public TNode GetEntityNode<TNode>(object entity)
        where TNode : Node
    {
        return (TNode)_nodeMappings[entity];
    }
}
