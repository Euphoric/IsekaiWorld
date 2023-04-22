using Godot;

namespace IsekaiWorld;

// ReSharper disable once InconsistentNaming
public partial class UIOverlay : Node2D
{
	private HexagonNode _mouseoverHexagon = null!;
	private GameUserInterface _gameUserInterface = null!;

	public void Initialize(GameUserInterface gameUserInterface)
	{
		_gameUserInterface = gameUserInterface;
	}
	
	public override void _Ready()
	{
		_mouseoverHexagon = new HexagonNode
		{
			Color = Colors.Red
		};
		AddChild(_mouseoverHexagon);
	}
	
	public override void _Process(double delta)
	{
		_mouseoverHexagon.HexPosition = _gameUserInterface.MouseHexPosition;
	}
}