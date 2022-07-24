using Godot;

public class GameNode : Node
{
    private  GameEntity _game;

    public GameEntity GameEntity => _game;
    public HexagonalMap MapNode { get; private set; }

    public override void _EnterTree()
    {
        _game = new GameEntity();

        base._EnterTree();
    }
    
    public override void _Ready()
    {
        var mapGenerator = new MapGenerator();
        //var mapGenerator = new WallTilingTestMapGenerator();
        //var mapGenerator = new ConstructionTestMapGenerator();
        _game.Initialize(mapGenerator);
        
        var adamCharacter = _game.AddCharacter("Adam");
        adamCharacter.Position = new HexCubeCoord(1, 1, -2);
        var eveCharacter = _game.AddCharacter("Eve");
        eveCharacter.Position = new HexCubeCoord(1, -1, 0);

        MapNode = GetNode<HexagonalMap>("Map/HexagonalMap");
    }

    public override void _Process(float delta)
    {
        _game.Update();
        _game.UpdateNodes(this);
        
        base._Process(delta);
    }
}
