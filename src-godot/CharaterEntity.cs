using System.Collections.Generic;
using System.Linq;
using Godot;

public class CharaterEntity
{
    private readonly HexagonalMap _map;
    private readonly HexagonPathfinding _pathfinding;

    public HexCubeCoord Position { get; set; }
    public bool IsIdle => _currentActivity == null && !_activityQueue.Any();
    
    private IActivity _currentActivity;
    private readonly Queue<IActivity> _activityQueue = new Queue<IActivity>();

    public CharaterEntity(HexagonalMap map, HexagonPathfinding pathfinding)
    {
        _map = map;
        _pathfinding = pathfinding;
    }

    public void Update(float delta)
    {
        if (_currentActivity == null)
        {
            if (_activityQueue.Any())
            {
                _currentActivity = _activityQueue.Dequeue();
                _map.RunActivity(_currentActivity);
            }
        }
        else if (_currentActivity.IsFinished)
        {
            _currentActivity = null;
        }
    }

    public void UpdateCharacterNode(HexagonNode node)
    {
        node.HexPosition = Position;
    }

    public void UpdatePathNode(Line2D node)
    {
        // var remainingPath = new[] { Position }.Concat(_movementQueue).ToArray();
        // node.Points = remainingPath.Select(hex => hex.Center(16)).ToArray();
    }

    public void Construct(ConstructionEntity construction)
    {
        var pathResult = _pathfinding.FindPath(Position, construction.Position);
        if (pathResult.Found)
        {
            IEnumerable<HexCubeCoord> movementPath = pathResult.Path;
            movementPath = movementPath.Take(movementPath.Count() - 1);
            
            _activityQueue.Enqueue(new MovementActivity(this, movementPath.ToList()));
            _activityQueue.Enqueue(new ConstructionActivity(_map, this, construction));
        }
    }
}