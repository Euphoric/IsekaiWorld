using Godot;
using System;
using System.Globalization;

public partial class FPSLabel : Label
{
	public override void _Process(double delta)
	{
		var fps = Engine.GetFramesPerSecond();
		this.Text = fps.ToString(CultureInfo.InvariantCulture);
	}
}
