using Godot;
using System;
using Ballonpopper.scripts;

namespace Ballonpopper.scenes;

public partial class Ballon_manager : Node3D
{
  [Export(Godot.PropertyHint.Range, "1, 10, 1")]
  public int ballon_count = 1;
  public int score = 0;

  public Godot.Label ui_points;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {

	foreach (var child in this.GetChildren())
	{
	  if (child is Balloon balloon)
	  {
		balloon.Popped += this.IncreaseScore;
	  }

	  if (child is Godot.Label label)
	  {
		this.ui_points = label;
	  }
	}
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public void IncreaseScore(int amount)
  {
	this.score += amount;
	Godot.GD.Print(score);
	this.ui_points.Text = $"Points: {this.score}";
  }
}
