using Godot;
using System;

public partial class Main : Node2D
{
    private const bool IsPaused = false;
    CanvasLayer PauseMenu;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PauseMenu = GetNode<CanvasLayer>("PauseMenu");
        PauseMenu.Visible = IsPaused;

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("PauseButton"))
        {
            TogglePuase();
        }
    }

	public void TogglePuase()
	{
		GetTree().Paused = !GetTree().Paused;
		
        if (GetTree().Paused == true)
        {
            PauseMenu.Visible = true;
        }
        else
        {
            PauseMenu.Visible = false;
        }
    }



}
