using System.Collections.Generic;
using System.Linq;

namespace IsekaiWorld.Game;

public class MovementActivity : Activity
{
    private readonly HexagonPathfinding _pathfinding;
    private readonly CharacterEntity _charater;
    private readonly IReadOnlyList<HexCubeCoord> _anyTargets;

    private float _movementTimer;
    private Queue<HexCubeCoord>? _movementQueue;

    public MovementActivity(GameEntity game, HexagonPathfinding pathfinding, CharacterEntity charater, HexCubeCoord target)
        :this(game, pathfinding, charater, new []{target})
    { }
    
    public MovementActivity(GameEntity game, HexagonPathfinding pathfinding, CharacterEntity charater, IReadOnlyList<HexCubeCoord> anyTargets)
        :base(game)
    {
        _pathfinding = pathfinding;
        _charater = charater;
        _anyTargets = anyTargets;
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
            var pathResult = _pathfinding.FindPathToAny(_charater.Position, _anyTargets);
            if (pathResult.Found)
            {
                foreach (var coord in pathResult.Path)
                {
                    _movementQueue.Enqueue(coord);
                }
            }
        }

        if (_movementQueue != null && _movementQueue.Any())
        {
            _movementTimer += 1;
            var moveSpeed = 3; // cells per second
            var delayBetweenCells = GameSpeed.BaseTps / moveSpeed;
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