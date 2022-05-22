using System.Collections.Generic;
using System.Linq;
using Godot;

public class CharacterEntity
{
    private readonly GameEntity _game;
    private readonly HexagonPathfinding _pathfinding;

    public HexCubeCoord Position { get; set; }
    public bool IsIdle => _currentActivity == null && !_activityQueue.Any();

    private IActivity _currentActivity;
    private readonly Queue<IActivity> _activityQueue = new Queue<IActivity>();

    public CharacterEntity(GameEntity game, HexagonPathfinding pathfinding)
    {
        _game = game;
        _pathfinding = pathfinding;
    }

    public void Update(float delta)
    {
        if (_currentActivity == null)
        {
            if (_activityQueue.Any())
            {
                _currentActivity = _activityQueue.Dequeue();
                _game.RunActivity(_currentActivity);
            }
        }
        else if (_currentActivity.IsFinished)
        {
            _currentActivity = null;
        }
    }

    public void Construct(ConstructionEntity construction)
    {
        _activityQueue.Enqueue(new MovementActivity(_pathfinding, this, construction.Position));
        _activityQueue.Enqueue(new ConstructionActivity(_game, this, construction));
    }

    private HexagonNode _node;

    public void InitializeNode(HexagonalMap map)
    {
        var characterHexagon = new HexagonNode
        {
            HexPosition = HexCubeCoord.Zero,
            Color = Colors.Blue
        };
        _node = characterHexagon;
        map.AddChild(characterHexagon);
    }

    public void UpdateNode()
    {
        _node.HexPosition = Position;
    }
}