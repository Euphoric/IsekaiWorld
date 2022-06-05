using Godot;
using System;

public class UserInterface : CanvasLayer
{
	private GameEntity _game;

	public override void _EnterTree()
	{
		var gameNode = GetNode<GameNode>("/root/GameNode");
		_game = gameNode.GameEntity;
		
		base._EnterTree();
	}

	// ReSharper disable once UnusedMember.Global
	public void _on_SelectionButton_pressed()
	{
		_game.UserInterface.SelectionToggled();
	}
	
	// ReSharper disable once UnusedMember.Global
	public void _on_ConstructionButton_pressed()
	{
		_game.UserInterface.ContructionToggled();
	}
}
