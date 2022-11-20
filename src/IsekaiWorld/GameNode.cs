using System;
using System.Diagnostics;
using Godot;

public partial class GameNode : Node
{
	private GameEntity _game = null!;
	private BuildingView _buildingView = null!;
	private MapItemView _mapItemView = null!;
	private CharacterView _characterView = null!;
	private UserInterface _userInterface = null!;
	private System.Threading.Thread _gameThread = null!;

	
	private readonly MessagingHub _viewMessagingHub = new MessagingHub();
	
	[Obsolete("Should not be accessible to view layer")]
	public GameEntity GameEntity => _game;

	public HexagonalMap MapNode { get; private set; } = null!;

	public override void _EnterTree()
	{
		_game = new GameEntity();
		_game.Messaging.ConnectMessageHub(_viewMessagingHub);

		_gameThread = new System.Threading.Thread(GameLoop);

		_characterView = new CharacterView(this);
		_viewMessagingHub.Register(_characterView.Messaging);
		_buildingView = new BuildingView(this);
		_viewMessagingHub.Register(_buildingView.Messaging);
		_mapItemView = new MapItemView(this);
		_viewMessagingHub.Register(_mapItemView.Messaging);
		_userInterface = GetNode<UserInterface>("UserInterface");
		_userInterface.Initialize(_game);
		_viewMessagingHub.Register(_userInterface.Messaging);
		MapNode = GetNode<HexagonalMap>("Map/HexagonalMap");
		_viewMessagingHub.Register(MapNode.Messaging);

		base._EnterTree();
	}

	public override void _Ready()
	{
		_gameThread.Start();
	}

	private void GameLoop()
	{
		var mapGenerator = new MapGenerator();
		//var mapGenerator = new WallTilingTestMapGenerator();
		//var mapGenerator = new ConstructionTestMapGenerator();
		_game.Initialize(mapGenerator);

		var adamCharacter = _game.AddCharacter("Adam");
		adamCharacter.Position = new HexCubeCoord(1, 1, -2);
		// var eveCharacter = _game.AddCharacter("Eve");
		// eveCharacter.Position = new HexCubeCoord(1, -1, 0);

		var maxTps = 60;
		var maxMs = 1000d / maxTps;
		
		Stopwatch watch = Stopwatch.StartNew();
		double lastUpdate = watch.Elapsed.TotalMilliseconds;
		int loopCounter = 0;
		while (true)
		{
			var startTicks = watch.Elapsed.TotalMilliseconds;
			
			_game.Update();
			
			while (watch.Elapsed.TotalMilliseconds - startTicks < maxMs)
			{ }

			loopCounter++;
			var now = watch.Elapsed.TotalMilliseconds;
			var milisecondsSinceLastUpdate = now - lastUpdate;
			if (milisecondsSinceLastUpdate > 500)
			{
				double tps = loopCounter / milisecondsSinceLastUpdate * 1000;
				
				loopCounter = 0;
				lastUpdate = now;
				
				_game.Messaging.Broadcast(new TpsChanged(tps));
			}
		}
	}
	
	public override void _Process(double delta)
	{
		_viewMessagingHub.DistributeMessages();
		
		_characterView.Update();
		_buildingView.Update();
		_mapItemView.Update();

		base._Process(delta);
	}
}
