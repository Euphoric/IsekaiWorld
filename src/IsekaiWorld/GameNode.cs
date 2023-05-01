using System;
using System.Diagnostics;
using Godot;

namespace IsekaiWorld;

public partial class GameNode : Node
{
	private GameEntity _game = null!;
	private BuildingView _buildingView = null!;
	private MapItemView _mapItemView = null!;
	private CharacterView _characterView = null!;
	private UserInterface _userInterface = null!;
	private System.Threading.Thread _gameThread = null!;

	
	private readonly MessagingHub _viewMessagingHub = new();
	
	[Obsolete("Should not be accessible to view layer")]
	public GameEntity GameEntity => _game;

	public GameUserInterface Gui { get; private set; } = null!;
	
	public Node2D EntitiesNode { get; private set; } = null!;

	public override void _EnterTree()
	{
		_game = new GameEntity();
		_game.MessagingHub.ConnectMessageHub(_viewMessagingHub);

		_gameThread = new System.Threading.Thread(GameLoop);

		Gui = new GameUserInterface();
		_viewMessagingHub.Register(Gui.Messaging);
		
		_characterView = new CharacterView(this);
		_viewMessagingHub.Register(_characterView.Messaging);
		_buildingView = new BuildingView(this);
		_viewMessagingHub.Register(_buildingView.Messaging);
		_mapItemView = new MapItemView(this);
		_viewMessagingHub.Register(_mapItemView.Messaging);
		_userInterface = GetNode<UserInterface>("UserInterface");
		_userInterface.Initialize(Gui);
		_viewMessagingHub.Register(_userInterface.Messaging);
		var mapNode = GetNode<HexagonalMap>("Map/HexagonalMap");
		_viewMessagingHub.Register(mapNode.Messaging);
		EntitiesNode = GetNode<Node2D>("Map/Entities");
		var uiOverlay = GetNode<UIOverlay>("Map/UIOverlay");
		uiOverlay.Initialize(Gui);
		
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
		var eveCharacter = _game.AddCharacter("Eve");
		eveCharacter.Position = new HexCubeCoord(1, -1, 0);

		Stopwatch watch = Stopwatch.StartNew();
		double lastUpdate = watch.Elapsed.TotalMilliseconds;
		int loopCounter = 0;
		while (true)
		{
			var startTicks = watch.Elapsed.TotalMilliseconds;
			
			_game.Update();
			
			var maxTps = 60 * _game.Speed;
			var maxMs = 1000d / maxTps;
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
				
				_game.MessagingHub.Broadcast(new TpsChanged(tps));
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

	public void RegisterMessaging(MessagingEndpoint messaging)
	{
		_viewMessagingHub.Register(messaging);
	}
}
