//* Libraries imports */
using Godot;
using System;

//* Local imports */
using Ballonpopper.scripts;

namespace Ballonpopper.scenes;

public partial class Ballon_manager : Node3D
{
  [Export(Godot.PropertyHint.Range, "1, 10, 1")]
  public int ballon_count = 1;
  [Export(Godot.PropertyHint.Range, "0.1, 10, 0.1")]
  public float minimal_spawn_delay = 0.2f;
  [Export(Godot.PropertyHint.Range, "0.1, 10, 0.1")]
  public float max_spawn_delay = 2.0f;
  [Export(Godot.PropertyHint.ResourceType, "Scene")]
  public Godot.PackedScene BallonScene; // modelo do balão

  public int score = 0;

  public Godot.Label ui_points;
  private Godot.Timer spawn_timer;

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

    this.spawn_timer = new Godot.Timer
    {
      WaitTime = GD.RandRange(this.minimal_spawn_delay, this.max_spawn_delay),
      Autostart = true,
    };

    this.AddChild(this.spawn_timer);
    this.spawn_timer.Timeout += this.SpawnBallon;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public void IncreaseScore(int amount)
  {
    this.score += amount;
    this.ui_points.Text = $"Points: {this.score}";
  }

  private void SpawnBallon()
  {
    if (this.BallonScene == null) return;

    var balloon = this.BallonScene.Instantiate<Balloon>();

    double randomX = GD.RandRange(-5.0, 5.0);
    double randomY = GD.RandRange(-0, 3.0);
    double randomZ = GD.RandRange(-2.0, 2.0);

    balloon.Position = new Godot.Vector3(
      (float)randomX,
      (float)randomY,
      (float)randomZ
    );

    balloon.Popped += IncreaseScore;
    this.AddChild(balloon);
  }
}
