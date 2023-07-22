using System.Collections.Generic;
using Godot;

namespace IsekaiWorld.View;

// ReSharper disable once InconsistentNaming
public partial class UIOverlay : Node2D
{
	private HexagonNode _mouseoverHexagon = null!;
	private GameUserInterface _gameUserInterface = null!;
	
	private readonly List<HexagonNode> _highlightHexes = new();
	
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

		foreach (var hexagonNode in _highlightHexes)
		{
			hexagonNode.Visible = false;
		}
		for (int i = 0; i < _gameUserInterface.HighlightedHexes.Count; i++)
		{
			var position = _gameUserInterface.HighlightedHexes[i];
			if (_highlightHexes.Count <= i)
			{
				var hexagonNode = new HexagonNode();
				hexagonNode.Color = Colors.LimeGreen.Lerp(Colors.Transparent, 0.6f);
				hexagonNode.InnerSize = 0f;
				hexagonNode.OuterSize = 1f;
				_highlightHexes.Add(hexagonNode);
				AddChild(hexagonNode);
			}

			_highlightHexes[i].HexPosition = position;
			_highlightHexes[i].Visible = true;
		}
	}
}