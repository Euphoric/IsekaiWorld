using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace IsekaiWorld;

public class MovementActivity : Activity
{
    private readonly HexagonPathfinding _pathfinding;
    private readonly CharacterEntity _charater;
    private readonly IReadOnlyList<HexCubeCoord> _anyTargets;

    private float _movementTimer;
    private Queue<HexCubeCoord>? _movementQueue;

    [Obsolete("Use constructor with targets list")]
    public MovementActivity(GameEntity game, HexagonPathfinding pathfinding, CharacterEntity charater, HexCubeCoord target, bool stopNextTo)
        :this(game, pathfinding, charater, AllTargets(target, stopNextTo))
    { }

    private static List<HexCubeCoord> AllTargets(HexCubeCoord target, bool stopNextTo)
    {
        var stopNeighbors = stopNextTo ? target.Neighbors() : ImmutableList<HexCubeCoord>.Empty;
        var targetAll = new List<HexCubeCoord> { target }.Concat(stopNeighbors).ToList();
        return targetAll;
    }

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