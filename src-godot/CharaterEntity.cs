using System.Collections.Generic;
using System.Linq;
using Godot;

public class CharaterEntity
{
    private readonly HexagonPathfinding _pathfinding;

    public HexCubeCoord Position { get; set; }
    private float _movementTimer;
    private readonly Queue<HexCubeCoord> _movementQueue = new Queue<HexCubeCoord>();

    public CharaterEntity(HexagonPathfinding pathfinding)
    {
        _pathfinding = pathfinding;
    }

    public void MoveTo(HexCubeCoord targetPosition)
    {
        _movementQueue.Clear();
        
        var pathResult = _pathfinding.FindPath(Position, targetPosition);
        if (pathResult.Found)
        {
            foreach (var coord in pathResult.Path)
            {
                _movementQueue.Enqueue(coord);
            }
        }
    }

    public void Update(float delta)
    {
        if (_movementQueue.Any())
        {
            _movementTimer += delta;
            var movementDelay = 1 / 3f;
            if (_movementTimer > movementDelay)
            {
                _movementTimer -= movementDelay;
                Position = _movementQueue.Dequeue();
            }
        }
    }

    public void UpdateCharacterNode(HexagonNode node)
    {
        node.HexPosition = Position;
    }

    public void UpdatePathNode(Line2D node)
    {
        var remainingPath = new []{ Position }.Concat(_movementQueue).ToArray();
        node.Points = remainingPath.Select(hex => hex.Center(16)).ToArray();
    }
}