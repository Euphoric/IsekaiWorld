using Godot;

public partial class GameNode : Node
{
	private GameEntity _game;
	private BuildingView _buildingView;
	private MapItemView _mapItemView;
	
	public GameEntity GameEntity => _game;
	public HexagonalMap MapNode { get; private set; }

	public override void _EnterTree()
	{
		_game = new GameEntity();
		_buildingView = new BuildingView(this);
		_game.Messaging.Register(_buildingView.Messaging);
		_mapItemView = new MapItemView(this);
		_game.Messaging.Register(_mapItemView.Messaging);
		
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
		// var eveCharacter = _game.AddCharacter("Eve");
		// eveCharacter.Position = new HexCubeCoord(1, -1, 0);

		MapNode = GetNode<HexagonalMap>("Map/HexagonalMap");
		_game.Messaging.Register(MapNode.Messaging);
	}

	public override void _Process(double delta)
	{
		_game.Update();
		_game.UpdateNodes(this);
		_buildingView.Update();
		_mapItemView.Update();
		
		base._Process(delta);
	}
}
