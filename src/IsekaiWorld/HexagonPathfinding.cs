using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace IsekaiWorld;

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
	public MessagingEndpoint Messaging { get; }

	private readonly Dictionary<HexCubeCoord, Node> _nodes = new();
	private readonly Dictionary<INode, HexCubeCoord> _nodeToHexPosition = new();

	public HexagonPathfinding()
	{
		Messaging = new MessagingEndpoint(MessageHandler);
	}
	
	public void Update()
	{
	}
	
	private void MessageHandler(IEntityMessage message)
	{
		switch (message)
		{
			case BuildingUpdated buildingUpdated:
				OnBuildingUpdated(buildingUpdated);
				break;
		}
	}

	public void BuildMap(HexagonalMapEntity hexagonalMapEntity)
	{
		var cells = hexagonalMapEntity.Cells;
		foreach (var cell in cells)
		{
			var hex = cell.Position;
			var hexCenter = hex.Center(1);
			var node = new Node(new Position(hexCenter.X, hexCenter.Y));
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

	public PathfindingResult FindPathToAny(HexCubeCoord from, IReadOnlyList<HexCubeCoord> toAny)
	{
		var path = toAny.Select(to =>
			{
				var fromNode = _nodes[from];
				var toNode = _nodes[to];

				var pathFinder = new PathFinder();
				return pathFinder.FindPath(fromNode, toNode, Velocity.FromKilometersPerHour(1));
			}).Where(path => path.Type == PathType.Complete)
			.MinBy(path => path.Distance);

		if (path == null)
		{
			return new PathfindingResult(false, ImmutableList<HexCubeCoord>.Empty);
		}

		var hexPath = path.Edges.Select(x => _nodeToHexPosition[x.End]);
		return new PathfindingResult(true, hexPath.ToList());
	}

	private void OnBuildingUpdated(BuildingUpdated message)
	{
		if (message.Definition.Impassable)
		{
			SetImpassable(message.Position);
		}
	}

	private void SetImpassable(HexCubeCoord position)
	{
		var centerNode = _nodes[position];
		var neighbors = position.Neighbors();
		foreach (var neighbor in neighbors)
		{
			var neighborNode = _nodes[neighbor];
			centerNode.Disconnect(neighborNode);
			neighborNode.Disconnect(centerNode);
		}
	}

	public bool IsPassable(HexCubeCoord position)
	{
		var centerNode = _nodes[position];
		return centerNode.Outgoing.Any();
	}
}