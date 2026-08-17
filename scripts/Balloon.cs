using Godot;
using System;

namespace Ballonpopper.scripts;

public partial class Balloon : Area3D
{

  [Godot.ExportGroup("Pop Settings")]
  [Godot.Export(Godot.PropertyHint.Range, "1,10,1")] // 1 to 10, step 1
  public int ClicksToPop = 5;
  [Godot.Export(Godot.PropertyHint.Range, "1,100,1")] // 1 to 100, step 1
  public int PointsToGive = 10;
  [Godot.Export(Godot.PropertyHint.Range, "1,10,0.1")] // 1 to 10, step 0.1
  public float SizeIncreaseFactor = 1.2f;
  [Godot.Export(Godot.PropertyHint.Range, "0.1,1,0.1")] // 0.1 to 1, step 0.1
  public float PopAnimationDuration = 0.25f;
  protected bool _canBePopped = true;
  protected Godot.AudioStream _popSound;
  protected Godot.AudioStream _inflateSound;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	Godot.GD.Print("Balloon ready");
	// `/assets/ballon_pop_sound_effect.ogg`
	this._popSound = GD.Load<Godot.AudioStream>("res://assets/ballon_pop_sound_effect.ogg");
	this._inflateSound = GD.Load<Godot.AudioStream>("res://assets/balloon_inflating.ogg");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public override void _InputEvent(Camera3D camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeIdx)
  {
	var isMouseEvent = @event is Godot.InputEventMouseButton;
	if (isMouseEvent)
	{
	  // print the event
	  if (IsLeftClickDown(@event as Godot.InputEventMouseButton) && this._canBePopped)
	  {
		this.ClicksToPop--;
		Godot.GD.Print("Left mouse button pressed, clicks to pop: " + this.ClicksToPop);
		if (this.ClicksToPop <= 0)
		{
		  Godot.GD.Print("Balloon popped");
		  this._canBePopped = false;
		  this.PopBalloonAnimation();
		}
		else
		{
		  this.IncreaseBalloonSize();
		}
	  }
	}
  }

  private bool IsLeftClickDown(Godot.InputEventMouseButton mouseButtonEvent)
  {
	return mouseButtonEvent.ButtonIndex == Godot.MouseButton.Left && mouseButtonEvent.IsPressed();
  }

  private void IncreaseBalloonSize()
  {
	var actualScale = this.Scale;
	var finalScale = new Vector3(
	  this.SizeIncreaseFactor * actualScale.X,
	  this.SizeIncreaseFactor * actualScale.Y,
	  this.SizeIncreaseFactor * actualScale.Z
	);
	var duration = this.PopAnimationDuration;

	this.IncreaseBalloonSizeTween(finalScale, duration);
	this.PlayInflateSound();
  }

  private async void PopBalloonAnimation()
  {
	var actualScale = this.Scale;
	var finalScale = new Vector3(
	  this.SizeIncreaseFactor * actualScale.X,
	  this.SizeIncreaseFactor * actualScale.Y,
	  this.SizeIncreaseFactor * actualScale.Z
	);
	var duration = this.PopAnimationDuration;

	await ToSignal(this.IncreaseBalloonSizeTween(finalScale, duration), Tween.SignalName.Finished);
	PlayPopSound();
	QueueFree();
  }

  private void PlayPopSound()
  {
	var soundPlayer = new Godot.AudioStreamPlayer
	{
	  Stream = this._popSound,
	};
	GetParent().AddChild(soundPlayer);
	soundPlayer.Finished += soundPlayer.QueueFree;
	soundPlayer.Play();
  }

  private void PlayInflateSound()
  {
	var soundPlayer = new Godot.AudioStreamPlayer
	{
	  Stream = this._inflateSound,
	};
	GetParent().AddChild(soundPlayer);
	soundPlayer.Finished += soundPlayer.QueueFree;
	soundPlayer.Play();
  }

  private Tween IncreaseBalloonSizeTween(Vector3 finalScale, float duration)
  {
	var property = Node3D.PropertyName.Scale.ToString();
	var tween = CreateTween();
	tween.TweenProperty(this, property, finalScale, duration)
	  .SetTrans(Tween.TransitionType.Sine)
	  .SetEase(Tween.EaseType.Out);

	return tween;
  }
}
