using System.Collections.Generic;
using System.Linq;

public class MovementActivity : Activity
{
    private readonly HexagonPathfinding _pathfinding;
    private readonly CharacterEntity _charater;
    private readonly HexCubeCoord _target;
    private readonly bool _stopNextTo;

    private float _movementTimer;
    private Queue<HexCubeCoord>? _movementQueue;

    public MovementActivity(GameEntity game, HexagonPathfinding pathfinding, CharacterEntity charater, HexCubeCoord target, bool stopNextTo)
        :base(game)
    {
        _pathfinding = pathfinding;
        _charater = charater;
        _target = target;
        _stopNextTo = stopNextTo;
    }

    protected override void UpdateInner()
    {
        if (_movementQueue != null && _movementQueue.Any())
        {
            var nextPosition = _movementQueue.Peek();
            var isNextCellPassable = _pathfinding.IsPassable(nextPosition);
            if (!isNextCellPassable)
            {
                _movementQueue = null;
            }
        }

        if (_movementQueue == null)
        {
            _movementQueue = new Queue<HexCubeCoord>();
            var pathResult = _pathfinding.FindPath(_charater.Position, _target);
            if (pathResult.Found)
            {
                IEnumerable<HexCubeCoord> movementPath = pathResult.Path;
                if (_stopNextTo)
                {
                    movementPath = movementPath.Take(movementPath.Count() - 1);
                }

                foreach (var coord in movementPath)
                {
                    _movementQueue.Enqueue(coord);
                }
            }
        }

        if (_movementQueue != null && _movementQueue.Any())
        {
            _movementTimer += 1;
            var delayBetweenCells = 15f;
            if (_movementTimer > delayBetweenCells)
            {
                _movementTimer -= delayBetweenCells;
                var nextPosition = _movementQueue.Dequeue();
                var facingDirection = _charater.Position.DirectionTo(nextPosition);
                _charater.Position = nextPosition;
                _charater.FacingDirection = facingDirection;
            }
        }
        
        IsFinished = _movementQueue != null && !_movementQueue.Any();
    }
}