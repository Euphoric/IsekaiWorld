using Godot;
using System;
using System.Globalization;

public class FPSLabel : Label
{
	public override void _Process(float delta)
	{
		var fps = Engine.GetFramesPerSecond();
		this.Text = fps.ToString(CultureInfo.InvariantCulture);
	}
}
