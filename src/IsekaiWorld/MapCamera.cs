using Godot;

namespace IsekaiWorld;

public partial class MapCamera : Camera2D
{
    public override void _Ready()
    {
        Zoom = Vector2.One * 4f;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("zoom_in"))
        {
            Zoom *= 0.95f;
        }
        if (Input.IsActionPressed("zoom_out"))
        {
            Zoom *= 1 / 0.95f;
        }
        
        base._Input(@event);
        
        UpdateLabel();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero;
        if (Input.IsActionPressed("move_right"))
        {
            velocity += Vector2.Right;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity += Vector2.Left;
        }

        if (Input.IsActionPressed("move_up"))
        {
            velocity += Vector2.Up;
        }

        if (Input.IsActionPressed("move_down"))
        {
            velocity += Vector2.Down;
        }

        float speed = 400;
        Position += velocity * speed * (float)delta * (Vector2.One / Zoom);

        base._Process(delta);
        
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        var label = GetNode<Label>("/root/GameNode/UserInterface/Container/DebugLabel");
        label.Text = Position + " / " + Zoom;
    }
}