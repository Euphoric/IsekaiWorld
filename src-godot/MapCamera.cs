using Godot;
using System;

public class MapCamera : Camera2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionPressed("zoom_in"))
		{
			Zoom = Zoom * (0.95f);
		}
		if (Input.IsActionPressed("zoom_out"))
		{
			Zoom = Zoom * (1 / (0.95f));
		}
		
		base._Input(@event);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_right"))
		{
			velocity -= Vector2.Right;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity -= Vector2.Left;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity -= Vector2.Up;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity -= Vector2.Down;
		}

		float speed = 100;
		Position += velocity * speed * delta;

		var label = GetNode<Label>("../UI/Container/DebugLabel");
		label.Text = Position + " / " + Zoom;
	}
}
