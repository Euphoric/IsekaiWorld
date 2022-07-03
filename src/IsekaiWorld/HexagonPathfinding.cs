using System.Collections.Generic;
using System.Linq;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

public class PathfindingResult
{
    public bool Found { get; }
    public IReadOnlyList<HexCubeCoord> Path { get; }

    public PathfindingResult(bool found, IReadOnlyList<HexCubeCoord> path)
    {
        Found = found;
        Path = path;
    }
}

public class HexagonPathfinding
{
    public EntityMessaging Messaging { get; } = new EntityMessaging();

    private readonly Dictionary<HexCubeCoord, Node> _nodes = new Dictionary<HexCubeCoord, Node>();
    private readonly Dictionary<INode, HexCubeCoord> _nodeToHexPosition = new Dictionary<INode, HexCubeCoord>();

    public HexagonPathfinding()
    {
        Messaging.Register<BuildingUpdated>(BuildingUpdated);
    }

    public void BuildMap(HexagonalMapEntity hexagonalMapEntity)
    {
        var cells = hexagonalMapEntity.Cells;
        foreach (var cell in cells)
        {
            var hex = cell.Position;
            var hexCenter = hex.Center(1);
            var node = new Node(new Position(hexCenter.x, hexCenter.y));
            _nodes[hex] = node;
            _nodeToHexPosition[node] = hex;
        }

        var hexPossToCell = cells.ToDictionary(x => x.Position, x => x);
        foreach (var cell in cells)
        {
            if (!cell.Surface.IsPassable)
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

                var neighborCell = hexPossToCell[neighbor];
                if (neighborCell.Surface.IsPassable)
                    hexNode.Connect(neightborNode, Velocity.FromKilometersPerHour(1));
            }
        }
    }

    public PathfindingResult FindPath(HexCubeCoord from, HexCubeCoord to)
    {
        var fromNode = _nodes[from];
        var toNode = _nodes[to];

        var pathFinder = new PathFinder();
        var path = pathFinder.FindPath(fromNode, toNode, Velocity.FromKilometersPerHour(1));
        var hexPath = path.Edges.Select(x => _nodeToHexPosition[x.End]);
        return new PathfindingResult(path.Type == PathType.Complete, hexPath.ToList());
    }

    private void BuildingUpdated(BuildingUpdated message)
    {
        if (message.Definition.Impassable)
        {
            SetImpassable(message.Position);
        }
    }

    private void SetImpassable(HexCubeCoord position)
    {
        var centerNode = _nodes[position];
        centerNode.Incoming.Clear();
        centerNode.Outgoing.Clear();
    }
}