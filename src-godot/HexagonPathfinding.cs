using System.Collections.Generic;
using System.Linq;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

public class HexagonPathfinding
{
    private readonly Dictionary<HexCubeCoord, Node> _nodes = new Dictionary<HexCubeCoord, Node>();
    private readonly Dictionary<INode, HexCubeCoord> _nodeToHexPosition = new Dictionary<INode, HexCubeCoord>();
    
    
    public void BuildMap(HexMap hexMap)
    {
        bool[] isPassable = new[]
        {
            false,
            true,
            true,
            false
        };
        
        var cells = hexMap.Cells;
        foreach (var cell in cells)
        {
            var hex = cell.Position;
            var hexCenter = hex.Center(1);
            var node = new Node(new Position(hexCenter.x, hexCenter.y));
            _nodes[hex] = node;
            _nodeToHexPosition[node] = hex;
        }

        foreach (var cell in cells)
        {
            if (!isPassable[cell.Surface])
                continue;
            
            var hex = cell.Position;
            var hexNode = _nodes[hex];
            var neighbors = hex.Neighbors();
            foreach (var neighbor in neighbors)
            {
                if (!_nodes.TryGetValue(neighbor, out var neightborNode))
                {
                    continue;
                }
                
                hexNode.Connect(neightborNode, Velocity.FromKilometersPerHour(1));
            }
        }
    }
    
    public IReadOnlyList<HexCubeCoord> FindPath(HexCubeCoord from, HexCubeCoord to)
    {
        var fromNode = _nodes[from];
        var toNode = _nodes[to];
        
        var pathFinder = new PathFinder();
        var path = pathFinder.FindPath(fromNode, toNode, Velocity.FromKilometersPerHour(1));
        var hexPath = path.Edges.Select(x => _nodeToHexPosition[x.End]);
        return new[] { from }.Concat(hexPath).ToList();
    }
}