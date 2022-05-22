using System.Collections.Generic;
using System.Linq;

public class MovementActivity : IActivity
{
    private readonly CharaterEntity _charater;
    
    private float _movementTimer;
    private readonly Queue<HexCubeCoord> _movementQueue = new Queue<HexCubeCoord>();

    public MovementActivity(HexagonPathfinding pathfinding, CharaterEntity charater, HexCubeCoord target)
    {
        _charater = charater;
        
        var pathResult = pathfinding.FindPath(charater.Position, target);
        if (pathResult.Found)
        {
            IEnumerable<HexCubeCoord> movementPath = pathResult.Path;
            movementPath = movementPath.Take(movementPath.Count() - 1);
            
            foreach (var coord in movementPath)
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
                _charater.Position = _movementQueue.Dequeue();
            }
        }
    }

    public bool IsFinished => !_movementQueue.Any();
}